import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddToCollectionModalComponent } from './addcard-modal.component';

describe('AddcardModalComponent', () => {
  let component: AddToCollectionModalComponent;
  let fixture: ComponentFixture<AddToCollectionModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddToCollectionModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddToCollectionModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
