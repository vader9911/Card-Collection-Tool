import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { CollectionsService } from '../../services/collections.service';
import { AuthService } from '../../services/auth.service';
import { Subscription } from 'rxjs'
import { AddToCollectionModalComponent } from '../addcard-modal/addcard-modal.component'

@Component({
  selector: 'app-card-details',
  standalone: true,
  imports: [
    CommonModule,
    AddToCollectionModalComponent

  ],
  templateUrl: './card-details.component.html',
  styleUrls: ['./card-details.component.scss']
})
export class CardDetailsComponent implements OnInit {
  cardId: any;
  cardDetails: any = null;
  isLoggedIn: boolean = false; // Track user authentication status
  authSubscription?: Subscription;
  selectedCardId: any | undefined;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private collectionsService: CollectionsService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    // Subscribe to the authentication status
    this.authSubscription = this.authService.isLoggedIn().subscribe((status) => {
      this.isLoggedIn = status;
    });
    // Get card ID from the route parameters
    this.cardId = this.route.snapshot.paramMap.get('cardId') ?? '';
    console.log(this.cardId);
    // Load card details if card ID is available
    if (this.cardId) {
      this.apiService.getCardDetails(this.cardId).subscribe(
        (details) => {
          this.cardDetails = details;
        },
        (error) => {
          console.error('Error fetching card details:', error);
        }
      );
    }
  }

  // Function to open the add-to-collection modal
  openAddToCollectionModal(cardId: number) {
    console.log("modal for add card opened");
    this.selectedCardId = cardId; // Store the selected card ID
    const modal = document.getElementById('addToCollectionModal');
    if (modal) {
      modal.style.display = 'block';
    }
  }
}
