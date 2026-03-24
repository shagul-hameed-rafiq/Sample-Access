import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PanelsService } from '../../../services/panels.service';
import { MembersService } from '../../../services/members.service';

@Component({
  standalone: true,
  imports: [CommonModule],
  templateUrl: './report-view.html',
  styleUrls: ['./report-view.scss']
})
export class ReportView implements OnInit {

  report: any = null;
  member: import('../../../models/member.model').Member | null = null;
  memberId!: number;
  visitId!: number;
  loading = true;
  errorMsg = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private panelsService: PanelsService,
    private membersService: MembersService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Use paramMap observable for better reliability
    this.route.paramMap.subscribe(params => {
      this.visitId = Number(params.get('reportId'));
      this.memberId = Number(this.route.parent?.snapshot.paramMap.get('id'));

      console.log('ReportView Init - visitId:', this.visitId, 'memberId:', this.memberId);

      if (!this.visitId || isNaN(this.visitId)) {
        this.loading = false;
        this.errorMsg = 'Invalid report ID.';
        this.cdr.detectChanges();
        return;
      }

      this.fetchMember();
      this.fetchReport();
    });
  }

  fetchMember(): void {
    this.membersService.getMemberById(this.memberId).subscribe({
      next: (m) => {
        this.member = m;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to fetch member details', err)
    });
  }

  fetchReport(): void {
    this.loading = true;
    this.errorMsg = '';
    this.report = null;
    this.cdr.detectChanges();

    // Timeout after 7 seconds if no response
    const timeoutTimer = setTimeout(() => {
      if (this.loading) {
        this.loading = false;
        this.errorMsg = 'Fetching report is taking too long. The server might be unreachable or the visit ID is invalid.';
        this.cdr.detectChanges();
      }
    }, 7000);

    this.panelsService.getVisitReport(this.visitId).subscribe({
      next: (data: any) => {
        clearTimeout(timeoutTimer);
        console.log('API Response for Visit Report:', data);

        if (!data || Object.keys(data).length === 0) {
          this.errorMsg = 'The visit was found, but the report content is empty.';
        } else {
          // Process report to prioritize revisions
          const processedReport = { ...data };

          // CRITICAL: If the API report is missing revisions, check if we have a "held" version in LocalStorage
          const localData = this.membersService.getMemberFromMemory(this.memberId);
          const heldReport = localData.reports.find((r: any) => Number(r.visitId) === Number(this.visitId));

          if (heldReport) {
            console.log('Found "held" version of report in local storage, merging revisions...');
            // Merge Panel Summary editions if API didn't return them
            if (heldReport.panelSummary) {
              processedReport.panelSummary = {
                ...processedReport.panelSummary,
                ...heldReport.panelSummary
              };
              // Ensure the view sees the edit
              if (heldReport.panelSummary.panelRevisedSummary) {
                processedReport.panelSummary.standardSummary = heldReport.panelSummary.panelRevisedSummary;
                processedReport.panelSummary.panelRevisedSummary = heldReport.panelSummary.panelRevisedSummary;
              }
            }

            // Merge Test editions
            if (heldReport.tests && processedReport.tests) {
              processedReport.tests = processedReport.tests.map((t: any) => {
                const heldTest = heldReport.tests.find((ht: any) => ht.testId === t.testId);
                return heldTest ? { ...t, ...heldTest } : t;
              });
            }
          }

          // Fallback mapping for display
          if (processedReport.panelSummary) {
            processedReport.panelSummary.standardSummary =
              processedReport.panelSummary.panelRevisedSummary ||
              processedReport.panelSummary.standardSummary;
          }

          if (processedReport.tests) {
            processedReport.tests = processedReport.tests.map((t: any) => ({
              ...t,
              standardReport: t.revisedReport || t.standardReport
            }));
          }

          this.report = processedReport;
        }

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        clearTimeout(timeoutTimer);
        this.loading = false;
        this.errorMsg = 'Could not retrieve report. Server returned an error (404/500).';
        console.error('Report fetch error:', err);
        this.cdr.detectChanges();
      }
    });
  }



  getSeverityLabel(severity: number): string {
    switch (severity) {
      case 0: return 'Normal';
      case 1: return 'Mild';
      case 2: return 'Moderate';
      case 3: return 'High';
      case 4: return 'Very High';
      default: return 'Unknown';
    }
  }

  getSeverityClass(severity: number): string {
    switch (severity) {
      case 0: return 'severity-normal';
      case 1: return 'severity-mild';
      case 2: return 'severity-moderate';
      case 3: return 'severity-high';
      case 4: return 'severity-very-high';
      default: return '';
    }
  }

  back(): void {
    this.router.navigate(['/members', this.memberId]);
  }

  printReport(): void {
    window.print();
  }

  calculateAge(dateOfBirth?: string): number {
    if (!dateOfBirth) return 0;

    const birthDate = new Date(dateOfBirth);
    if (isNaN(birthDate.getTime())) return 0;

    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (
      monthDiff < 0 ||
      (monthDiff === 0 && today.getDate() < birthDate.getDate())
    ) {
      age--;
    }

    return age < 0 ? 0 : age;
  }
}
