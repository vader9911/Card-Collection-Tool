import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CollectionsService } from '../../services/collections.service'
import { ReactiveFormsModule, FormsModule } from '@angular/forms';


@Component({
  selector: 'app-addcard-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './addcard-modal.component.html',
  styleUrl: './addcard-modal.component.css'
})
export class AddToCollectionModalComponent implements OnInit {
  @Input() cardId?: string | undefined; // Card ID passed to the modal
  collections: any[] = []; // List of collections
  selectedCollectionId: number | null = null; // Initialize selected collection ID
  quantity: number = 1;

  constructor(private collectionsService: CollectionsService) { }

  ngOnInit(): void {
    this.loadCollections(); // Load collections when the modal opens
  }

  // Fetch collections from the server
  loadCollections() {
    this.collectionsService.getCollections().subscribe((collections) => {
      this.collections = collections;
      console.log('Loaded collections:', this.collections);
    });
  }

  // Add card to selected collection
  addToCollection() {
    console.log('addToCollection called'); // Log to confirm the method is called
    console.log('Selected Collection ID:', this.selectedCollectionId); // Log the selected collection ID
    console.log('Quantity:', this.quantity); // Log the quantity
    console.log('Card ID:', this.cardId); // Log the card ID
    if (this.selectedCollectionId && this.cardId && this.quantity > 0) {
      console.log('Making request to add card:', this.cardId, 'with quantity:', this.quantity);

      this.collectionsService.addCardToCollection(this.selectedCollectionId, this.cardId, this.quantity).subscribe(
        () => {
          //alert('Card added to collection successfully!');
          this.closeModal(); // Close the modal after adding the card
        },
        (error) => {
          console.error('Error adding card to collection:', error);
          alert('Failed to add card to collection. Please try again.');
        }
      );
    } else {
      alert('Please select a collection, enter a valid quantity, and ensure a card is selected.');
    }
  }

  // Create a new collection and add the card to it
  createAndAddToCollection(collectionName: string) {
    if (collectionName.trim()) {
      console.log('Making request to add card:', this.cardId)
      this.collectionsService.createCollection(collectionName).subscribe(
        (newCollection) => {
          this.collectionsService.addCardToCollection(newCollection.id, this.cardId!, this.quantity).subscribe(
            () => {
              alert('New collection created and card added successfully!');
              this.closeModal();
            },
            (error) => {
              console.error('Error adding card to new collection:', error);
              alert('Failed to add card to the new collection. Please try again.');
            }
          );
        },
        (error) => {
          console.error('Error creating new collection:', error);
          alert('Failed to create a new collection. Please try again.');
        }
      );
    } else {
      alert('Please enter a collection name.');
    }
  }

  // Close the modal
  closeModal() {
    const modal = document.getElementById('addToCollectionModal');
    if (modal) {
      modal.style.display = 'none';
    }
  }
}
