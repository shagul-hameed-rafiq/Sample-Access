import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface BackendPanel {
  panelId: number;
  panelName: string;
  panelCode: string;
  testCount: number;
}

@Injectable({
  providedIn: 'root',
})
export class PanelsService {
  private baseUrl = '/api/panels';

  constructor(private http: HttpClient) { }

  getPanels(): Observable<BackendPanel[]> {
    return this.http.get<BackendPanel[]>(this.baseUrl);
  }
  getTestsByPanel(panelId: number) {
    return this.http.get<any[]>(`/api/panels/${panelId}/tests`);
  }
  getAllTests() {
    return this.http.get<any[]>('/api/tests');
  }
  createVisit(memberId: number, payload: any) {
    return this.http.post(`/api/members/${memberId}/visits`, payload, {
      responseType: 'text',
    });
  }
  getTests(panelId: number) {
    return this.http.get<any[]>(`${this.baseUrl}/${panelId}/tests`);
  }

  getVisitReport(visitId: number) {
    return this.http.get<any>(`/api/visits/${visitId}/report`);
  }

  evaluateVisit(visitId: number) {
    // Changing from null to {} as some .NET backends require an object to avoid 400 errors
    return this.http.post<any>(`/api/visits/${visitId}/evaluate`, {});
  }



  finalizeVisit(visitId: number, body: { panelRevisedSummary: string; testRevisions: { testId: number; revisedReport: string }[] }) {
    return this.http.post(`/api/visits/${visitId}/finalize`, body);
  }

  deleteVisit(memberId: number, visitId: number) {
    // Correcting to a more likely member-scoped route for this backend
    return this.http.delete(`/api/members/${memberId}/visits/${visitId}`);
  }
}


