import { Injectable } from '@angular/core';
import { ReportIntake } from '../models/report-intake.model';
import { LabTest } from '../models/test.model';

@Injectable({
  providedIn: 'root',
})
export class ReportDraftService {

  private intake?: ReportIntake;
  private panelId: number | null = null;
  private tests: LabTest[] = [];

  // -----------------------
  // Intake
  // -----------------------

  setIntake(data: ReportIntake): void {
    this.intake = data;
  }

  getIntake(): ReportIntake | undefined {
    return this.intake;
  }

  // -----------------------
  // Panel
  // -----------------------

  setPanel(panelId: number): void {
    this.panelId = panelId;
  }

  getPanel(): number | null {
    return this.panelId;
  }

  // -----------------------
  // Tests
  // -----------------------

  setTests(tests: LabTest[]): void {
    this.tests = tests;
  }

  getTests(): LabTest[] {
    return this.tests;
  }

  // -----------------------
  // Visit ID
  // -----------------------

  private visitId: number | null = null;

  setVisitId(id: number): void {
    this.visitId = id;
  }

  getVisitId(): number | null {
    return this.visitId;
  }

  // -----------------------
  // Report
  // -----------------------

  private report: any = null;

  setReport(report: any): void {
    this.report = report;
  }

  getReport(): any {
    return this.report;
  }

  // -----------------------
  // Clear Entire Draft
  // -----------------------

  clearDraft(): void {
    this.intake = undefined;
    this.tests = [];
    this.panelId = null;
    this.visitId = null;
    this.report = null;
  }
}