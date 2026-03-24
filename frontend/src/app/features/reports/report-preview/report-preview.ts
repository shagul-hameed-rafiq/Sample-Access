import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ReportDraftService } from '../../../services/report-draft.service';
import { PanelsService } from '../../../services/panels.service';
import { MembersService } from '../../../services/members.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './report-preview.html',
  styleUrls: ['./report-preview.scss'],
})
export class ReportPreview implements OnInit {
  memberId!: number;
  visitId!: number;
  report: any = null;

  // Editable fields
  panelRevisedSummary = '';
  tests: { testId: number; testName: string; rawValue: string; bandName: string; severity: number; revisedReport: string }[] = [];

  finalizing = false;
  finalized = false;
  loading = false;
  errorMsg = '';

  constructor(
    private draft: ReportDraftService,
    private panelsService: PanelsService,
    private membersService: MembersService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Parent memberId
    this.route.parent?.paramMap.subscribe(params => {
      this.memberId = Number(params.get('id'));
    });

    // Subscribing to paramMap for reliable visitId tracking
    this.route.paramMap.subscribe(params => {
      this.visitId = Number(params.get('visitId'));

      if (!this.visitId) {
        this.errorMsg = 'Invalid Visit ID.';
        this.cdr.detectChanges();
        return;
      }

      this.loadReport();
    });
  }

  loadReport(): void {
    this.loading = true;
    this.cdr.detectChanges();

    this.panelsService.getVisitReport(this.visitId).subscribe({
      next: (data: any) => {
        console.log('Report Preview Data Received:', data);
        this.report = data;
        this.initializePreviewFields();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMsg = 'Failed to fetch report from server.';
        this.loading = false;
        console.error('Preview Load Error:', err);
        this.cdr.detectChanges();
      }
    });
  }


  initializePreviewFields(): void {
    if (!this.report) return;

    // Initialize editable fields - prioritize existing revisions if they exist
    this.panelRevisedSummary =
      this.report.panelSummary?.panelRevisedSummary ||
      this.report.panelSummary?.standardSummary || '';

    this.tests = (this.report.tests ?? []).map((t: any) => ({
      testId: t.testId,
      testName: t.testName,
      rawValue: t.rawValue,
      bandName: t.bandName,
      severity: t.severity,
      revisedReport: t.revisedReport || t.standardReport || '',
    }));
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

  finalize(): void {
    if (!this.visitId) return;
    this.finalizing = true;
    this.errorMsg = '';

    const body = {
      panelRevisedSummary: this.panelRevisedSummary,
      testRevisions: this.tests.map(t => ({
        testId: t.testId,
        revisedReport: t.revisedReport,
      })),
    };

    this.panelsService.finalizeVisit(this.visitId, body).subscribe({
      next: () => {
        this.finalizing = false;
        this.finalized = true;

        // Also save to local storage as a backup to make it "held" on the profile
        const localReport = {
          ...this.report,
          visitId: this.visitId,
          visitDateTime: new Date().toISOString(),
          status: 'Finalized',
          // Merge the revisions into the local representation
          panelSummary: {
            ...this.report.panelSummary,
            standardSummary: this.panelRevisedSummary,
            panelRevisedSummary: this.panelRevisedSummary
          },
          tests: this.report.tests.map((t: any) => {
            const revision = this.tests.find(rev => rev.testId === t.testId);
            return {
              ...t,
              standardReport: revision ? revision.revisedReport : t.standardReport,
              revisedReport: revision ? revision.revisedReport : t.revisedReport
            };
          })
        };
        this.membersService.addReportToMember(this.memberId, localReport);

        this.draft.clearDraft();

        // Navigate back to member details after short delay
        setTimeout(() => {
          this.router.navigate(['/members', this.memberId]);
        }, 1200);
      },

      error: (err) => {
        this.finalizing = false;
        this.errorMsg = 'Failed to finalize report. Please try again.';
        console.error('Finalize error:', err);
      },
    });
  }

  back(): void {
    window.history.back();
  }
}
