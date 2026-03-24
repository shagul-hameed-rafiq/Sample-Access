import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddReportIntake } from './add-report-intake';

describe('AddReportIntake', () => {
  let component: AddReportIntake;
  let fixture: ComponentFixture<AddReportIntake>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddReportIntake]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddReportIntake);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
