import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportView } from './report-view';

describe('ReportView', () => {
  let component: ReportView;
  let fixture: ComponentFixture<ReportView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReportView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReportView);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
