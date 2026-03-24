import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MembersService } from '../../../services/members.service';
import { OnInit, ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-add-member',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-member.html',
  styleUrls: ['./add-member.scss'],
})
export class AddMember implements OnInit {

  member = {
    name: '',
    gender: 'Male' as 'Male' | 'Female' | 'Other',
    dateOfBirth: '',
    bloodGroup: 0,
    contact: '',
    address: '',
  };

  saving = false;
  isEdit = false;
  memberId: number | null = null;
  errorMessage = '';

  constructor(
    private membersService: MembersService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isEdit = true;
        this.memberId = +id;

        // Try to pre-fill from service if we already have the member "held" in memory
        const heldMember = this.membersService.getCurrentMember();
        if (heldMember && heldMember.memberId === this.memberId) {
          this.populateForm(heldMember);
        }

        // Still fetch from backend to ensure we have the absolute latest data
        this.fetchMember(this.memberId);
      }
    });
  }

  fetchMember(id: number) {
    this.membersService.getMemberById(id).subscribe({
      next: (data) => {
        if (data) {
          this.populateForm(data);
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load member data';
        this.cdr.detectChanges();
      }
    });
  }

  populateForm(data: any) {
    if (!data) return;
    this.member.name = data.name || '';
    this.member.gender = (data.gender as any) || 'Male';
    this.member.dateOfBirth = data.dateOfBirth ? data.dateOfBirth.split('T')[0] : '';
    this.member.bloodGroup = this.unmapBloodGroup(data.bloodGroup);
    this.member.contact = data.contact || '';
    this.member.address = data.address || '';

    this.cdr.detectChanges();
  }

  saveMember() {
    this.saving = true;
    this.errorMessage = '';

    // Use toISOString() as it was in the original working Add Member code
    let formattedDate = '';
    try {
      formattedDate = new Date(this.member.dateOfBirth).toISOString();
    } catch (e) {
      formattedDate = this.member.dateOfBirth;
    }

    const payload: any = {
      name: this.member.name,
      gender: this.member.gender,
      dateOfBirth: formattedDate,
      bloodGroup: this.mapBloodGroup(this.member.bloodGroup),
      contact: Number(this.member.contact), // Convert to number as per Swagger
      address: this.member.address,
    };

    console.log('Attempting save with payload:', payload);

    // Using addMember directly to ensure we use the "Create Member API" (POST /api/members)
    this.membersService.addMember(payload).subscribe({
      next: () => {
        this.saving = false;
        if (this.isEdit && this.memberId) {
          this.router.navigate(['/members', this.memberId]);
        } else {
          this.router.navigate(['/members']);
        }
      },
      error: (err) => {
        console.error('Save failed:', err);
        this.saving = false;
        this.errorMessage = 'Failed to save member profile. Please check console for details.';
        this.cdr.detectChanges();
      }
    });
  }

  mapBloodGroup(value: number): string {
    return this.bloodGroups[value] || 'O_Positive';
  }

  unmapBloodGroup(group: string): number {
    const idx = this.bloodGroups.indexOf(group);
    return idx === -1 ? 6 : idx; // Default to O_Positive (6)
  }

  bloodGroups = [
    'A_Positive',
    'A_Negative',
    'B_Positive',
    'B_Negative',
    'AB_Positive',
    'AB_Negative',
    'O_Positive',
    'O_Negative',
  ];

  cancel() {
    this.router.navigate(['/members']);
  }
}
