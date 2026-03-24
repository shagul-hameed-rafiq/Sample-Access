export type TestStatus = 'LOW' | 'NORMAL' | 'HIGH';

export interface TestInterpretation {
  testCode: string;
  status: TestStatus;
  text: string;
}

export interface PanelSummary {
  panelCode: string;
  statusCode: string;
  summary: string;
}
