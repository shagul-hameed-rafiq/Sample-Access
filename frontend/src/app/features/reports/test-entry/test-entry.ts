import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PanelsService } from '../../../services/panels.service';
import { ReportDraftService } from '../../../services/report-draft.service';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-test-entry',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './test-entry.html',
  styleUrls: ['./test-entry.scss'],
})
export class TestEntryComponent implements OnInit {
  tests: any[] = [];
  allAvailableTests: any[] = [];
  loading = false;
  generatingReport = false;
  panelId!: number;
  memberId!: number;
  errorMsg = '';
  selectedExtraTestId: number | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private panelsService: PanelsService,
    private draft: ReportDraftService,
    private cdr: ChangeDetectorRef,
    private location: Location,
  ) { }

  goBack(): void {
    this.router.navigate(['/members', this.memberId, 'reports', 'add'], {
      queryParams: { step: '2' }
    });
  }

  trackById(index: number, item: any): number {
    return item.testId;
  }

  ngOnInit(): void {
    this.panelId = Number(this.route.snapshot.paramMap.get('panelId'));
    this.memberId = Number(this.route.parent?.snapshot.paramMap.get('id'));

    if (!this.panelId) return;
    this.loadTests();
    this.loadAllTests();
  }

  loadAllTests(): void {
    this.panelsService.getAllTests().subscribe({
      next: (data: any[]) => {
        this.allAvailableTests = data;
        this.cdr.detectChanges();
      }
    });
  }

  addSupplementalTest(): void {
    if (!this.selectedExtraTestId) return;

    const testToAdd = this.allAvailableTests.find(t => t.testId == this.selectedExtraTestId);
    if (testToAdd) {
      // Check if already in current list
      const exists = this.tests.find(t => t.testId === testToAdd.testId);
      if (!exists) {
        this.tests.push({ ...testToAdd, value: '' });
        this.cdr.detectChanges();
      } else {
        this.errorMsg = 'This test is already in the list.';
        setTimeout(() => this.errorMsg = '', 3000);
      }
    }
    this.selectedExtraTestId = null;
  }

  loadTests(): void {
    this.loading = true;
    this.panelsService.getTestsByPanel(this.panelId).subscribe({
      next: (data: any[]) => {
        this.tests = data.map((test) => ({ ...test, value: '' }));
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  generateReport(): void {
    this.generatingReport = true;
    this.errorMsg = '';
    const intake = this.draft.getIntake();

    // Validation: Check if all tests have values (Strict check)
    const missing = this.tests.filter(t => !t.value || String(t.value).trim() === '');
    if (missing.length > 0) {
      this.generatingReport = false;
      this.errorMsg = 'Please enter values for all test items before continuing.';
      this.cdr.detectChanges();
      return;
    }

    // Step 1: POST test values (create visit)
    const payload = {
      panelId: this.panelId,
      visitDateTime: new Date().toISOString(),
      height: intake?.heightCm ?? 0,
      weight: intake?.weightKg ?? 0,
      systolic: intake?.systolic ?? 0,
      diastolic: intake?.diastolic ?? 0,
      notes: '',
      results: this.tests.map((t) => ({
        testId: t.testId,
        rawValue: String(t.value).trim(),
      })),
    };


    this.panelsService.createVisit(this.memberId, payload).pipe(
      // Step 2: POST evaluate with the visitId from step 1
      switchMap((response: string) => {
        // Robust extraction: Handle JSON objects, raw numbers, or text messages with IDs
        let visitId: number;
        try {
          const parsed = JSON.parse(response);
          visitId = Number(parsed.visitId || parsed.id || (typeof parsed === 'number' ? parsed : null));
        } catch {
          const match = response.match(/\d+/);
          visitId = match ? Number(match[0]) : NaN;
        }

        console.log('Response from CreateVisit:', response);
        console.log('Extracted Visit ID:', visitId);

        if (!visitId || isNaN(visitId)) {
          console.error('Invalid ID found in response:', response);
          throw new Error('Could not determine Visit ID from server response');
        }


        console.log('Visit created, visitId:', visitId);
        this.draft.setVisitId(visitId);

        // Pass visitId along with the evaluate result
        return this.panelsService.evaluateVisit(visitId).pipe(
          switchMap(report => [{ report, visitId }])
        );
      })
    ).subscribe({
      next: ({ report, visitId }) => {
        this.generatingReport = false;
        this.draft.setReport(report);
        this.router.navigate(['/members', this.memberId, 'reports', 'preview', visitId]);
      },


      error: (err) => {
        this.generatingReport = false;
        this.errorMsg = 'Failed to generate report. Check console for details.';
        console.error('--- Generate Report Error ---');
        console.error('Status:', err.status);
        console.error('Body:', err.error);
        console.error('-----------------------------');
        this.cdr.detectChanges();
      },

    });
  }
}
