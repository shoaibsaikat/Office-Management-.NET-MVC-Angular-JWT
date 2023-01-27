import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PendingDistributionComponent } from './pending-distribution.component';

describe('PendingDistributionComponent', () => {
  let component: PendingDistributionComponent;
  let fixture: ComponentFixture<PendingDistributionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PendingDistributionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PendingDistributionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
