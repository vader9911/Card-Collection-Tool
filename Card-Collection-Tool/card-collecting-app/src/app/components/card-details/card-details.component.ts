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
  selectedCardId: any;
  cardDetails: any = null;
  isLoggedIn: boolean = false; // Track user authentication status
  authSubscription?: Subscription;
  pageCardId: any | undefined;

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
    this.selectedCardId = this.route.snapshot.paramMap.get('cardId') ?? '';
    console.log(this.selectedCardId, "this is the component log");
    // Load card details if card ID is available
    if (this.selectedCardId) {
      this.apiService.getCardDetails(this.selectedCardId).subscribe(
        (details) => {
          this.cardDetails = details;
          console.log('Card Details:', this.cardDetails);
        },
        (error) => {
          // Log detailed error information
          console.error('Error fetching card details:', error);

          // Log specific details if available
          if (error.status) {

            console.error(`HTTP Status: ${error.status}`);
          }
          if (error.message) {
            console.error(`Error Message: ${error.message}`);
          }
          if (error.error) {
            console.error('Error Details:', error.error);
          }
        }
      );
    }
  }

  // Function to open the add-to-collection modal
  openAddToCollectionModal(selectedCardId: number) {
    console.log("modal for add card opened");
    this.pageCardId = selectedCardId; // Store the selected card ID
    const modal = document.getElementById('addToCollectionModal');
    if (modal) {
      modal.style.display = 'block';
    }
  }
}
