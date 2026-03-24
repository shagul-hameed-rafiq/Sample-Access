import { LabTest } from './test.model';
import { TestInterpretation, PanelSummary } from './interpretation.model';
import { ReportIntake } from './report-intake.model';

export interface FinalReport {
  id: string;
  memberId: string;
  panelCode: string;
  createdAt: Date;
  status: 'FINAL';

  intake: ReportIntake;
  tests: LabTest[];
  interpretations: TestInterpretation[];
  summary: PanelSummary;
}
