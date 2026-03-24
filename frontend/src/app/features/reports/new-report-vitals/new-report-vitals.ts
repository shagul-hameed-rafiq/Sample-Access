import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MembersService } from '../../../services/members.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-new-report-vitals',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './new-report-vitals.html',
  styleUrls: ['./new-report-vitals.scss'],
})
export class NewReportVitals {
  height!: number;
  weight!: number;
  systolic!: number;
  diastolic!: number;

  constructor(
    private membersService: MembersService,
    private router: Router,
  ) {}

  saveVitals() {
    const member = this.membersService.getCurrentMember();
    if (!member) return;

    // Save vitals into member object
    this.membersService.updateVitals({
      height: this.height,
      weight: this.weight,
      systolicBP: this.systolic,
      diastolicBP: this.diastolic,
    });

    // Move to next step
    this.router.navigate(['/reports/new/panel']);
  }
  ngOnInit() {
    const member = this.membersService.getCurrentMember();
    console.log('Current member:', member);

    if (!member) {
      this.router.navigate(['/members']);
    }
  }
}
