import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Member } from '../models/member.model';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private apiUrl = '/api/members';

  private currentMember: Member | null = null;
  private currentReportDraft: any = null;

  constructor(private http: HttpClient) { }

  // =========================
  // BACKEND METHODS
  // =========================

  getMembers(): Observable<Member[]> {
    return this.http.get<Member[]>(this.apiUrl);
  }

  getMemberById(id: number): Observable<Member> {
    return this.http.get<Member>(`${this.apiUrl}/${id}`);
  }

  addMember(payload: any): Observable<any> {
    return this.http.post(this.apiUrl, payload);
  }

  updateMember(id: number, payload: any): Observable<any> {
    // Hitting the base creation endpoint as requested ("call the create member api")
    // The payload contains the memberId to tell the backend to update instead of create.
    return this.http.post(this.apiUrl, payload);
  }

  // =========================
  // CURRENT MEMBER STATE
  // =========================

  setCurrentMember(member: Member): void {
    this.currentMember = member;
  }

  getCurrentMember(): Member | null {
    return this.currentMember;
  }

  // =========================
  // REPORT DRAFT
  // =========================

  initializeReportDraft(): void {
    if (!this.currentMember) return;

    this.currentReportDraft = {
      memberId: this.currentMember.memberId,
      vitals: null,
      panelId: null,
      testValues: [],
    };
  }

  getReportDraft(): any {
    return this.currentReportDraft;
  }

  clearReportDraft(): void {
    this.currentReportDraft = null;
  }

  // =========================
  // SAVE REPORT (LOCALSTORAGE)
  // =========================

  // =========================
  // SAVE/REMOVE REPORT (LOCALSTORAGE)
  // =========================

  addReportToMember(memberId: number, report: any): void {
    const stored = JSON.parse(localStorage.getItem('memberReports') || '{}');

    if (!stored[memberId]) {
      stored[memberId] = [];
    }

    // Check if it's already there (avoid duplicates)
    const exists = stored[memberId].some((r: any) => Number(r.visitId) === Number(report.visitId));
    if (!exists) {
      stored[memberId].push(report);
      localStorage.setItem('memberReports', JSON.stringify(stored));
    }
  }

  removeReportFromMember(memberId: number, visitId: number): void {
    const stored = JSON.parse(localStorage.getItem('memberReports') || '{}');

    if (stored[memberId]) {
      stored[memberId] = stored[memberId].filter((r: any) => Number(r.visitId) !== Number(visitId));
      localStorage.setItem('memberReports', JSON.stringify(stored));
    }
  }

  finalizeReport(report: any): void {
    this.addReportToMember(report.memberId, report);
  }

  updateVitals(vitalsData: any): void {
    if (!this.currentReportDraft) {
      console.warn('No report draft initialized. Call initializeReportDraft() first.');
      return;
    }
    this.currentReportDraft.vitals = vitalsData;
  }
  getMemberVisits(memberId: number) {
    return this.http.get<any[]>(`${this.apiUrl}/${memberId}/visits`);
  }
  getMemberFromMemory(memberId: number): any {
    const storedReports = JSON.parse(localStorage.getItem('memberReports') || '{}');

    return {
      memberId: memberId,
      reports: storedReports[memberId] || [],
    };
  }
}
