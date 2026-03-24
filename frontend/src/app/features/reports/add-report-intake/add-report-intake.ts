import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportDraftService } from '../../../services/report-draft.service';
import { PanelsService, BackendPanel } from '../../../services/panels.service';

@Component({
  selector: 'app-add-report-intake',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-report-intake.html',
  styleUrls: ['./add-report-intake.scss']
})
export class AddReportIntake implements OnInit {

  memberId!: number;
  form!: FormGroup;
  currentStep = 1; // 1: Vitals, 2: Panels
  panels: BackendPanel[] = [];
  selectedPanelId: number | null = null;
  isLoadingPanels = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private draft: ReportDraftService,
    private panelsService: PanelsService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.memberId = Number(this.route.snapshot.paramMap.get('id'));

    this.form = this.fb.group({
      heightCm: [null, [Validators.required, Validators.min(30)]],
      weightKg: [null, [Validators.required, Validators.min(1)]],
      systolic: [null, [Validators.required, Validators.min(50)]],
      diastolic: [null, [Validators.required, Validators.min(30)]]
    });

    // Restore data from draft if available
    const existingIntake = this.draft.getIntake();
    if (existingIntake) {
      this.form.patchValue(existingIntake);
    }

    const existingPanelId = this.draft.getPanel();
    if (existingPanelId) {
      this.selectedPanelId = existingPanelId;
    }

    // Check if we should start at step 2 (Panel Selection)
    const step = this.route.snapshot.queryParamMap.get('step');
    if (step === '2') {
      this.currentStep = 2;
    }

    this.loadPanels();
  }

  loadPanels(): void {
    this.isLoadingPanels = true;
    this.panelsService.getPanels().subscribe({
      next: (res) => {
        this.panels = res;
        this.isLoadingPanels = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load panels', err);
        this.isLoadingPanels = false;
        this.cdr.detectChanges();
      }
    });
  }

  goToStep2(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.currentStep = 2;
    this.cdr.detectChanges();
  }

  goToStep1(): void {
    this.currentStep = 1;
    this.cdr.detectChanges();
  }

  selectPanel(panelId: number): void {
    this.selectedPanelId = panelId;
  }

  next(): void {
    if (!this.selectedPanelId) return;

    // Save vitals
    this.draft.setIntake({
      heightCm: this.form.value.heightCm,
      weightKg: this.form.value.weightKg,
      systolic: this.form.value.systolic,
      diastolic: this.form.value.diastolic
    });

    // Save panel
    this.draft.setPanel(this.selectedPanelId);

    // Navigate to Step 3 (Test Value Entry)
    this.router.navigate(['/members', this.memberId, 'reports', 'tests', this.selectedPanelId]);
  }

  cancel(): void {
    this.router.navigate(['/members', this.memberId]);
  }
}

