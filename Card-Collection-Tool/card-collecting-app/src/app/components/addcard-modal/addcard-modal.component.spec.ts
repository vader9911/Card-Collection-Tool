import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddcardModalComponent } from './addcard-modal.component';

describe('AddcardModalComponent', () => {
  let component: AddcardModalComponent;
  let fixture: ComponentFixture<AddcardModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddcardModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddcardModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
