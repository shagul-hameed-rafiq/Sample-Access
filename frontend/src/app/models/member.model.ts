export interface Member {
  memberId: number;
  name: string;
  gender: string;
  dateOfBirth?: string;  // present in GET /api/members/:id
  age?: number;          // present in GET /api/members (list)
  contact: string;
  address: string;
  bloodGroup: string;

  selectedPanelId?: number;

  vitals?: {
    height: number;
    weight: number;
    systolicBP: number;
    diastolicBP: number;
  };

  reports?: any[];
}

