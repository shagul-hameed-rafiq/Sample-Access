import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DummyReportEngineService {
  private panels = [
    { panelId: 1, panelName: 'Diabetic' },
    { panelId: 2, panelName: 'Thyroid' },
    { panelId: 3, panelName: 'CBC Panel' },
  ];

  private testsByPanel: { [key: number]: any[] } = {
    1: [
      { testId: 1, testName: 'HbA1c', unit: '%', min: 4, max: 5.6 },
      { testId: 2, testName: 'FBS', unit: 'mg/dL', min: 70, max: 100 },
    ],
    2: [
      { testId: 3, testName: 'TSH', unit: 'uIU/mL', min: 0.4, max: 4.0 },
      { testId: 4, testName: 'T3 (Total)', unit: 'ng/mL', min: 0.8, max: 2.1 },
      { testId: 5, testName: 'T4 (Total)', unit: 'µg/dL', min: 5.0, max: 14.0 },
    ],
    3: [{ testId: 6, testName: 'Hemoglobin', unit: 'g/dL', min: 13, max: 17 }],
  };

  // =========================
  // PANEL METHODS
  // =========================

  getPanels() {
    return this.panels;
  }

  getTestsForPanel(panelId: number) {
    return this.testsByPanel[panelId] || [];
  }

  // =========================
  // ADD TEST TO SPECIFIC PANEL
  // =========================

  addTestToPanel(panelId: number, test: any) {
    if (!this.testsByPanel[panelId]) {
      this.testsByPanel[panelId] = [];
    }

    const panelTests = this.testsByPanel[panelId];

    const newId = panelTests.length > 0 ? Math.max(...panelTests.map((t) => t.testId)) + 1 : 1;

    const newTest = {
      testId: newId,
      testName: test.testName,
      unit: test.unit,
      min: test.min,
      max: test.max,
    };

    panelTests.push(newTest);
  }

  // =========================
  // REPORT GENERATION
  // =========================

  generateReport(panelId: number, inputs: any[]) {
    const panel = this.panels.find((p) => p.panelId === panelId);
    const tests = this.testsByPanel[panelId] || [];

    let highestSeverity = 0;

    const detailedResults = tests.map((test) => {
      const input = inputs.find((i) => i.testId === test.testId);
      const value = input?.value ?? 0;

      let severity = 0;
      let interpretation = '---';

      // Only apply logic if test has min/max
      if (test.min !== undefined && test.max !== undefined) {
        if (value < test.min) {
          severity = 2;
          interpretation = `${test.testName} is below normal range.`;
        } else if (value > test.max) {
          severity = 2;
          interpretation = `${test.testName} is above normal range.`;
        } else {
          severity = 0;
          interpretation = `${test.testName} is within normal range.`;
        }
      }

      if (severity > highestSeverity) {
        highestSeverity = severity;
      }

      return {
        testName: test.testName,
        value: value,
        unit: test.unit,
        severity: severity,
        interpretation: interpretation,
      };
    });

    let overallSummary = '';

    if (highestSeverity === 0) {
      overallSummary = 'All test values are within normal range.';
    } else if (highestSeverity === 1) {
      overallSummary = 'Mild abnormalities detected.';
    } else {
      overallSummary = 'Significant abnormalities detected. Medical evaluation advised.';
    }

    return {
      panelName: panel?.panelName,
      highestSeverity: highestSeverity,
      summary: overallSummary,
      results: detailedResults,
    };
  }

  private generateDefaultInterpretation(test: any, value: number): string {
    if (value < test.min) {
      return `${test.testName} is below normal range.`;
    }

    if (value > test.max) {
      return `${test.testName} is above normal range.`;
    }

    return `${test.testName} is within normal range.`;
  }
}
