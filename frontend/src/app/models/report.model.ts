export interface Report {
  id: string;
  memberId: string;
  panelName: string;
  createdAt: Date;
  status: 'DRAFT' | 'FINAL';
}
