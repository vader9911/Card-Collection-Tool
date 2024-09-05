import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardSearchComponent } from '../card-search/card-search.component';
import { AuthService } from '../../services/auth.service';
import { Subscription } from 'rxjs'
import { AddToCollectionModalComponent } from '../addcard-modal/addcard-modal.component'

@Component({
  selector: 'app-card-list',
  standalone: true,
  imports: [
    CommonModule,
    CardListComponent,
    AddToCollectionModalComponent
    
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
  constructor(private authService: AuthService) {}

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

  openAddToCollectionModal(cardId: number) {
    const modal = document.getElementById('addToCollectionModal');
    if (modal) {
      modal.style.display = 'block';
    }

    // Set the cardId for the modal
    const modalComponent = document.querySelector('app-add-to-collection-modal') as any;
    if (modalComponent) {
      modalComponent.cardId = cardId;
    }
  }
}
