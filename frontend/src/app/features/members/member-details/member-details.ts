import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MembersService } from '../../../services/members.service';
import { PanelsService } from '../../../services/panels.service';
import { Member } from '../../../models/member.model';


@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './member-details.html',
  styleUrls: ['./member-details.scss'],
})
export class MemberDetails implements OnInit {

  member: Member | null = null;
  visits: any[] = [];   // ✅ NEW
  isLoading = true;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private membersService: MembersService,
    private panelsService: PanelsService,
    private cdr: ChangeDetectorRef
  ) { }


  goBack(): void {
    this.router.navigate(['/members']);
  }


  ngOnInit(): void {
    // Subscribe to paramMap to handle route changes within the same component
    this.route.paramMap.subscribe(params => {
      const memberId = Number(params.get('id'));

      if (!memberId) {
        this.isLoading = false;
        return;
      }

      // 🏎️ INSTANT PRE-FILL: Check if we already held this member in memory
      const held = this.membersService.getCurrentMember();
      if (held && held.memberId === memberId) {
        this.member = held;
        this.isLoading = false;
        this.cdr.detectChanges();
      }

      this.fetchMemberAndVisits(memberId);
    });
  }

  fetchMemberAndVisits(memberId: number): void {
    this.isLoading = true;

    // ✅ GET MEMBER FROM BACKEND
    this.membersService.getMemberById(memberId).subscribe({
      next: (data: Member) => {
        this.member = data;
        this.isLoading = false;

        // ✅ LOAD VISITS FROM BACKEND
        this.loadVisits(memberId);
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }


  // ✅ NEW METHOD: Load visits and merge with local "held" reports
  loadVisits(memberId: number) {
    this.membersService.getMemberVisits(memberId).subscribe({
      next: (data: any) => {
        console.log('Visits loaded from API:', data);
        const apiVisits = Array.isArray(data) ? data : (data?.visits || []);

        // Get local storage reports as well (the "held" ones)
        const stored = JSON.parse(localStorage.getItem('memberReports') || '{}');
        const localVisits = stored[memberId] || [];

        // Merge and deduplicate by visitId
        const mergedMap = new Map();

        // Add API visits first
        apiVisits.forEach((v: any) => mergedMap.set(v.visitId, v));

        // Add Local visits (backup)
        localVisits.forEach((v: any) => {
          if (!mergedMap.has(v.visitId)) {
            mergedMap.set(v.visitId, v);
          } else {
            const existing = mergedMap.get(v.visitId);
            if (v.status === 'Finalized') existing.status = 'Finalized';
          }
        });

        // ✅ STRICT FILTER: ONLY SHOW FINALIZED REPORTS (Remove 'Submitted', 'Draft', etc.)
        this.visits = Array.from(mergedMap.values())
          .filter(v => {
            const s = (v.status || '').toLowerCase();
            return s === 'finalized';
          })
          .sort((a, b) => Number(b.visitId) - Number(a.visitId));

        this.cdr.detectChanges();



      },
      error: (err) => {
        console.error('Error loading visits', err);
      }
    });
  }




  addReport(member: Member): void {
    this.membersService.setCurrentMember(member);
    this.membersService.initializeReportDraft();

    this.router.navigate([
      '/members',
      member.memberId,
      'reports',
      'add'
    ]);
  }

  goToEdit(): void {
    if (!this.member) return;
    this.membersService.setCurrentMember(this.member);
    this.router.navigate(['/members/edit', this.member.memberId]);
  }

  openReport(visitId: number): void {
    if (!this.member) return;

    this.router.navigate([
      '/members',
      this.member.memberId,
      'reports',
      'view',
      visitId
    ]);
  }

  deleteVisit(visitId: number): void {
    if (!this.member) return;
    const mId = this.member.memberId;

    if (confirm('Are you sure you want to delete this report?')) {
      this.panelsService.deleteVisit(mId, visitId).subscribe({
        next: () => {
          // 1. Remove from local storage cache
          this.membersService.removeReportFromMember(mId, visitId);

          // 2. Remove from local UI array immediately so it vanishes right away
          this.visits = this.visits.filter(v => Number(v.visitId) !== Number(visitId));

          alert('Report deleted successfully.');
          this.cdr.detectChanges();

          // 3. Sync with server in background
          this.loadVisits(mId);
        },
        error: (err) => {
          console.error('Delete failed:', err);
          // If already gone from server (404), clean local anyway
          if (err.status === 404) {
            this.membersService.removeReportFromMember(mId, visitId);
            this.visits = this.visits.filter(v => Number(v.visitId) !== Number(visitId));
            this.cdr.detectChanges();
          } else {
            alert('Failed to delete report. Please try again.');
          }
        }
      });
    }
  }




  getVitalsSummary(visit: any): string {
    if (!visit) return '---';
    const wt = visit.weight ? `${visit.weight}kg` : '';
    const ht = visit.height ? `${visit.height}cm` : '';
    const bp = (visit.systolic || visit.diastolic) ? `BP: ${visit.systolic || 0}/${visit.diastolic || 0}` : '';

    return [wt, ht, bp].filter(x => !!x).join(' / ');
  }

  getAvatarInitial(name?: string): string {
    if (!name) return '?';
    return name.charAt(0).toUpperCase();
  }

  calculateAge(dateOfBirth?: string): number {
    if (!dateOfBirth) return 0;

    const birthDate = new Date(dateOfBirth);
    if (isNaN(birthDate.getTime())) return 0;

    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (
      monthDiff < 0 ||
      (monthDiff === 0 && today.getDate() < birthDate.getDate())
    ) {
      age--;
    }

    return age < 0 ? 0 : age;
  }
}
