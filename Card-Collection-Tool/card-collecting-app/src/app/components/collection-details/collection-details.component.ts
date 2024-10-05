import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { CollectionsService } from '../../services/collections.service';
import { CardDetailModalComponent } from '../../components/card-detail-modal/card-detail-modal.component';

import * as bootstrap from 'bootstrap';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-collection-details',
  standalone: true,
  imports: [
    CommonModule,
    CardDetailModalComponent
  ],
  templateUrl: './collection-details.component.html',
  styleUrls: ['./collection-details.component.scss']
})
export class CollectionDetailsComponent implements OnInit {
  isLoggedIn: boolean = false;
  collectionDetails: any = null;
  authSubscription?: Subscription;
  routeSubscription?: Subscription;
  collectionId: number = 0;
  displayFormat: string = 'grid'; // Default display format is 'grid'
  deleteModal: any;
  selectedCardId: string | undefined;
  selectedCardName: string | undefined;
  showModal: boolean = false;
  @ViewChild(CardDetailModalComponent) cardDetailModal!: CardDetailModalComponent;
  constructor(
    private authService: AuthService,
    private collectionsService: CollectionsService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap.subscribe(params => {
      this.collectionId = Number(params.get('collectionId'));
      console.log('Collection ID from route:', this.collectionId);

      if (isNaN(this.collectionId) || this.collectionId <= 0) {
        console.error('Invalid collection ID:', this.collectionId);
        this.router.navigate(['/']); // Redirect to default route if invalid
        return;
      }

      this.authSubscription = this.authService.isLoggedIn().subscribe((status) => {
        this.isLoggedIn = status;
        if (this.isLoggedIn) {
          this.loadCollectionDetails();
        } else {
          this.router.navigate(['/login']); // Redirect to login if not authenticated
        }
      });
    });
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }

  // Method to open the delete confirmation modal
  openDeleteModal(): void {
    const deleteModalElement = document.getElementById('deleteModal');
    if (deleteModalElement) {
      this.deleteModal = new bootstrap.Modal(deleteModalElement);
      this.deleteModal.show();
    }
  }

  // Method to delete the collection
  deleteCollection(): void {
    // Hide the modal explicitly before deletion
    if (this.deleteModal) {
      this.deleteModal.hide();
    }

    this.collectionsService.deleteCollection(this.collectionId).subscribe(
      () => {
        console.log('Collection deleted successfully.');
        this.router.navigate(['/collections']); // Redirect to collections list after deletion
      },
      (error) => {
        console.error('Error deleting collection:', error);
        alert('An unexpected error occurred while deleting the collection.');
      }
    );
  }

  loadCollectionDetails(): void {
    if (this.collectionId) {
      this.collectionsService.getCollectionDetails(this.collectionId).subscribe(
        (details) => {
          this.collectionDetails = details;
          console.log('Collection details loaded:', this.collectionDetails);
        },
        (error) => {
          console.error('Error loading collection details:', error);
        }
      );
    } else {
      console.error('Invalid collection ID');
    }
  }

  removeCard(cardId: string): void {
    console.log(cardId);
    this.collectionsService.removeCardFromCollection(this.collectionId, cardId).subscribe(
      () => {
        // Update the collection details after the card is removed
        this.loadCollectionDetails();
      },
      error => {
        console.error('Error removing card:', error);
      }
    );
  }


  updateQuantity(cardId: string | undefined, quantityChange: number): void {
   console.log(cardId)
    this.collectionsService.updateCardQuantity(this.collectionId, cardId, quantityChange).subscribe(
      () => {
        
        // Update the collection details after the quantity is changed
        this.loadCollectionDetails();

        
      },
      error => {
        console.error('Error updating card quantity:', error);
      }
    );
  }



  // Method to navigate to card details page
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

  // Method to toggle the display format
  toggleDisplayFormat(format: string): void {
    this.displayFormat = format;
  }
}
