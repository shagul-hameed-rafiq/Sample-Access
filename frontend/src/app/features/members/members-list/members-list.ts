import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MembersService } from '../../../services/members.service';
import { Member } from '../../../models/member.model';

@Component({
  selector: 'app-members-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './members-list.html',
  styleUrls: ['./members-list.scss'],
})
export class MembersList implements OnInit {

  members: Member[] = [];
  loading = true;

  constructor(
    private membersService: MembersService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.membersService.getMembers().subscribe({
      next: (data) => {
        this.members = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  openMember(member: Member): void {
    this.membersService.setCurrentMember(member);
    this.router.navigate(['/members', member.memberId]);
  }

  addMember(): void {
    this.router.navigate(['/members/add']);
  }

  /**
   * Calculates age from a dateOfBirth string.
   * Adjusts for whether the birthday has passed this year.
   */
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

  /**
   * Returns the member's age.
   * The GET /api/members list endpoint returns a pre-computed `age` field
   * but does NOT include `dateOfBirth`. This helper uses `age` from the API
   * directly, falling back to calculateAge(dateOfBirth) if available.
   */
  getAge(member: Member): number {
    if (member.dateOfBirth) {
      return this.calculateAge(member.dateOfBirth);
    }
    return member.age ?? 0;
  }

  getAvatarInitial(name?: string): string {
    if (!name) return '?';
    return name.charAt(0).toUpperCase();
  }
}
