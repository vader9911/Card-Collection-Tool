import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { CollectionsService } from '../../services/collections.service';
import { ApiService } from '../../services/api.service';
import { CardDetailModalComponent } from '../../components/card-detail-modal/card-detail-modal.component';
import { Collection } from '../../models/collection';
import * as bootstrap from 'bootstrap';
import { Subscription } from 'rxjs';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators  } from '@angular/forms';

@Component({
  selector: 'app-collection-details',
  standalone: true,
  imports: [
    CommonModule,
    CardDetailModalComponent,
    ReactiveFormsModule

    
  ],
  templateUrl: './collection-details.component.html',
  styleUrls: ['./collection-details.component.scss']

})
export class CollectionDetailsComponent implements OnInit {
  isLoggedIn: boolean = false;
  collections: Collection[] = [];
  collectionDetails: any | null;
  authSubscription?: Subscription;
  routeSubscription?: Subscription;
  collectionId: number = 0;
  collectionName: string | undefined;
  displayFormat: string = 'grid'; // Default display format is 'grid'
  deleteModal: any;
  selectedCardId: string | undefined;
  selectedCardName: string | undefined;
  showModal: boolean = false;
  @ViewChild(CardDetailModalComponent) cardDetailModal!: CardDetailModalComponent;
  editCollectionForm!: FormGroup | undefined; // Form group for editing the collection
  editModal: any;

  constructor(
    private authService: AuthService,
    private collectionsService: CollectionsService,
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
   

  ) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.removeModalBackdrops();
      }
    });
  }

  ngOnInit(): void {
  
    this.routeSubscription = this.route.paramMap.subscribe(params => {
      this.collectionId = Number(params.get('collectionId'));
      console.log('Collection ID from route:', this.collectionId);
      this.attachRemoveModalHandler();
      
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

    this.editCollectionForm = this.fb.group({
      collectionName: ['', Validators.required],
      collectionImage: [''],
      notes: ['']
    });
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    this.cleanUpModal();
   
  }


  loadCollections(): void {
    this.collectionsService.getCollections().subscribe(
      (response) => {
        this.collections = response; // Store the fetched collections
        console.log('Loaded collections:', this.collections);
      },
      (error) => {
        console.error('Error fetching collections:', error);
      }
    );
  }

  // Method to open the delete confirmation modal
  openDeleteModal(): void {
    const deleteModalElement = document.getElementById('deleteModal');
    if (deleteModalElement) {
      const deleteModal = new bootstrap.Modal(deleteModalElement);
      deleteModal.show();
      if (this.editModal) {
        this.editModal.hide();
      }
    }
  }

  // Method to delete the collection
  deleteCollection(): void {
    this.closeDeleteModal();
    this.collectionsService.deleteCollection(this.collectionId).subscribe(
      () => {
        console.log('Collection deleted successfully.');
        this.cleanUpModal();
        this.router.navigate(['/collections']);
      },
      (error) => {
        console.error('Error deleting collection:', error);
        alert('An unexpected error occurred while deleting the collection.');
      }
    );
  }

  // Method to close the delete modal and hide any artifacts
  private closeDeleteModal(): void {
    if (this.deleteModal) {
      this.deleteModal.hide();
    }
    this.cleanUpModal();
  }

  // Method to clean up modal artifacts and restore scrolling
  private cleanUpModal(): void {
    document.body.classList.remove('modal-open');

    const backdrops = document.getElementsByClassName('modal-backdrop');
    while (backdrops.length > 0) {
      backdrops[0].parentNode?.removeChild(backdrops[0]);
    }
    document.body.style.overflow = 'auto';
  }

  private enablePageScroll(): void {
    document.body.classList.remove('modal-open');
  }

  private removeModalBackdrops(): void {
    // Remove all modal backdrops
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach((backdrop) => backdrop.remove());

    // Remove any overlay or other elements that may prevent interaction
    const overlays = document.querySelectorAll('.modal-open');
    overlays.forEach((overlay) => overlay.classList.remove('modal-open'));
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

  removeCard(cardId: string | undefined): void {
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

  removeCardButton(cardId: string | undefined): void {
    console.log(cardId);
    this.showRemoveModal(cardId)
  }


  updateCardQuantity(cardId: string | undefined, quantityChange: number): void {
    const card = this.collectionDetails.cards.find((c: { cardID: string | undefined; }) => c.cardID === cardId);

    console.log(card);
    if (card.quantity == 1 && quantityChange == -1) {
      this.showRemoveModal(cardId)

    } else {
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
  }

  // Show the modal to confirm card removal
  showRemoveModal(cardId: string | undefined): void {
    this.selectedCardId = cardId;
    const modalElement = document.getElementById('removeModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement); // Use Bootstrap's modal
      modal.show();
    } else {
        console.log('Error showing modal');
    }
    
    
  }

  // Attach handler for remove button click
  attachRemoveModalHandler(): void {
    const removeButton = document.getElementById('confirmRemoveBtn');
    if (removeButton) {
      removeButton.addEventListener('click', () => {
        this.confirmRemove();
      });
    }
  }

  // Confirm removal of the card
  confirmRemove(): void {
    if (this.selectedCardId) {
      console.log(this.selectedCardId);
      this.removeCard(this.selectedCardId);
    }

    const modalElement = document.getElementById('removeModal');
    if (modalElement) {
      const modalInstance = bootstrap.Modal.getInstance(modalElement); // Retrieve existing instance

      if (modalInstance) {
        modalInstance.hide(); // Hide the modal after confirming
      }

      // Remove any remaining modal backdrop manually
      const backdrops = document.querySelectorAll('.modal-backdrop');
      backdrops.forEach((backdrop) => {
        backdrop.remove();
      });
    }
  }

  openEditModal(): void {

    if (!this.collectionDetails || !this.collectionDetails.cards || !this.editCollectionForm) {
      console.error('Collection details or cards are not loaded yet.');
      return; // Exit if the data is not ready
    }

    const modalElement = document.getElementById('editModal');
    if (modalElement) {
      this.editModal = new bootstrap.Modal(modalElement);
      this.editModal.show();

      // Pre-fill the form with existing values
      this.editCollectionForm.patchValue({
        collectionName: this.collectionDetails.collectionDetails.collectionName,
        collectionImage: this.collectionDetails.collectionDetails.imageUri || this.collectionDetails.cards[0]?.imageUri, // Safely access cards[0]
        notes: this.collectionDetails.collectionDetails.notes || ''
      });
    }
  }

  // Save the changes made to the collection
  saveCollectionEdits(): void {
    if (this.editCollectionForm) {
      const updatedData = this.editCollectionForm.value;

      const collectionId = this.collectionId;
      const collectionName = updatedData.collectionName;
      const imageUri = updatedData.collectionImage;
      const notes = updatedData.notes;
      console.log(notes);

      this.collectionsService.updateCollection(collectionId, collectionName, imageUri, notes).subscribe(
        () => {
          console.log('Collection updated successfully');
          this.editModal.hide();  // Hide the modal after successful update
          this.loadCollectionDetails();  // Refresh the collection details
        },
        (error) => {
          console.error('Error updating collection:', error);
        }
      );
    }
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
