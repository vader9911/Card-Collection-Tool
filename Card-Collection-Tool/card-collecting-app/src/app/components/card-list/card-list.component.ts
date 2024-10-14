import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import {  Router } from '@angular/router';
import { CardSearchComponent } from '../card-search/card-search.component';
import { AuthService } from '../../services/auth.service';
import { Subscription } from 'rxjs'
import { AddToCollectionModalComponent } from '../addcard-modal/addcard-modal.component'
import { CardDetailModalComponent } from '../card-detail-modal/card-detail-modal.component';



@Component({
  selector: 'app-card-list',
  standalone: true,
  imports: [
    CommonModule,
    CardListComponent,
    AddToCollectionModalComponent,
    CardDetailModalComponent,
    
  ],
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.scss']
})
export class CardListComponent implements OnInit {
  @Input() cards: any[] = [];
  @Input() searchPerformed: boolean = false;
  @Input() noResultsReturned: boolean = false;

  isLoggedIn: boolean = false; // Track user authentication status
  authSubscription?: Subscription;
  selectedCardId: string | undefined;
  selectedCardName: string | undefined;
  showModal: boolean = false;
  @ViewChild(CardDetailModalComponent) cardDetailModal!: CardDetailModalComponent;
  @ViewChild(AddToCollectionModalComponent) addToCollectionModal!: AddToCollectionModalComponent;
  constructor( private authService: AuthService, private router: Router ) {}


  ngOnInit(): void {
    // Subscribe to the authentication status
    this.authSubscription = this.authService.isLoggedIn().subscribe((status) => {
      this.isLoggedIn = status;
    });
  }

  ngOnDestroy(): void {
    // Unsubscribe from the observable when the component is destroyed
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }


  openCardDetailModal(cardId: string | undefined, cardName: string | undefined): void {
    console.log('Card clicked with ID:', cardId, cardName);

    // Set the card details
    this.selectedCardId = cardId;
    this.selectedCardName = cardName;

    // Call the open method on the modal component
    if (this.cardDetailModal) {
      this.cardDetailModal.openModal(this.selectedCardId, this.selectedCardName);
    }
  }

  closeCardDetailModal(): void {
    this.showModal = false;
    const modal = document.getElementById('cardDetailModal');
    if (modal) {
      modal.style.display = 'none';
    }
  }

  openAddToCollectionModal(cardId: string | undefined, cardName: string | undefined) {
  console.log("Modal for add card opened with ID:", cardId); // Log card ID
   this.selectedCardId = cardId; 
   this.selectedCardName = cardName;
   if (this.addToCollectionModal) {
     this.addToCollectionModal.openModal(this.selectedCardId, this.selectedCardName);
   }
}
}
