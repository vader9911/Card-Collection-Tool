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
  @Input() cardId?: number; // Card ID passed to the modal
  collections: any[] = []; // List of collections
  selectedCollectionId: number | null = null; // Initialize selected collection ID

  constructor(private collectionsService: CollectionsService) { }

  ngOnInit(): void {
    this.loadCollections(); // Load collections when the modal opens
  }

  // Fetch collections from the server
  loadCollections() {
    this.collectionsService.getCollections().subscribe((collections) => {
      this.collections = collections;
    });
  }

  // Add card to selected collection
  addToCollection() {
    if (this.selectedCollectionId) {
      this.collectionsService.addCardToCollection(this.selectedCollectionId, this.cardId!).subscribe(() => {
        alert('Card added to collection successfully!');
        this.closeModal();
      });
    } else {
      alert('Please select a collection.');
    }
  }

  // Create a new collection and add the card to it
  createAndAddToCollection(collectionName: string) {
    if (collectionName.trim()) {
      this.collectionsService.createCollection(collectionName).subscribe((newCollection) => {
        this.collectionsService.addCardToCollection(newCollection.id, this.cardId!).subscribe(() => {
          alert('New collection created and card added successfully!');
          this.closeModal();
        });
      });
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
