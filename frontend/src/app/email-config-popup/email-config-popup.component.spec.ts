import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailConfigPopupComponent } from './email-config-popup.component';

describe('EmailConfigPopupComponent', () => {
  let component: EmailConfigPopupComponent;
  let fixture: ComponentFixture<EmailConfigPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmailConfigPopupComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmailConfigPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
