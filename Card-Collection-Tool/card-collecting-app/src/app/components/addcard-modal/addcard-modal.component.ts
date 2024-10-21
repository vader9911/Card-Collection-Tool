import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CollectionsService } from '../../services/collections.service';
import { ApiService } from '../../services/api.service';
import { GroupByPipe } from '../../shared/group-by.pipe';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-addcard-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    GroupByPipe
  ],
  templateUrl: './addcard-modal.component.html',
  styleUrls: ['./addcard-modal.component.scss']
})
export class AddToCollectionModalComponent implements OnInit {
  @Input() selectedCardId: string | undefined; // Card ID passed to the modal
  @Input() selectedCardName: string | undefined;
  @Input() selectedCardImage: string | undefined;
  cardId: string | undefined;
  cardName: string | undefined;
  cardImage: string | undefined;
  cardDetails: any;
  collections: any[] = []; // List of collections
  alternateVersions: any[] = [];
  selectedCollectionId: number | null = null;
  quantity: number = 1;
  addCollectIsOpen: boolean = false;

  constructor(private collectionsService: CollectionsService, private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadCollections(); // Load collections when the modal opens
    this.cardId = this.selectedCardId;
    this.cardName = this.selectedCardName;
    this.cardImage = this.selectedCardImage
    if (this.cardId && this.cardName) {
      this.fetchCardDetails(this.cardId);
      this.fetchAlternateVersions(this.cardName);
    }
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
    if (this.selectedCollectionId && this.cardId && this.quantity > 0) {
      const collectionID = Number(this.selectedCollectionId);
      this.collectionsService.addCardToCollection(collectionID, this.cardId, this.quantity, this.cardImage ).subscribe(
        () => {
          this.closeModal();
        },
        (error) => {
          console.error('Error adding card to collection:', error);
        }
      );
    }
  }

  // Create a new collection and add the card to it
  createAndAddToCollection(collectionName: string) {
    if (collectionName.trim()) {
      this.collectionsService.createCollection(collectionName).subscribe(
        (newCollection) => {
          // Ensure newCollection.id exists before proceeding
          if (newCollection?.collectionID) {
            this.collectionsService.addCardToCollection(newCollection.collectionID, this.cardId!, this.quantity, this.cardImage).subscribe(
              () => {
                this.loadCollections()
                this.closeModal();
              },
              (error) => {
                console.error('Error adding card to new collection:', error);
              }
            );
          } else {
            console.error('New collection created but no ID was returned');
          }
        },
        (error) => {
          console.error('Error creating new collection:', error);
        }
      );
    }
  }



  switchVersion(versionId: string, versionName: string, versionImage: string): void {
    console.log('Version clicked:', versionId, versionName, versionImage);
    this.cardId = versionId;
    this.cardName = versionName;
    this.cardImage = versionImage;
    
    this.fetchCardDetails(versionId); // Fetch details for the clicked version
  }

  // Fetch card details by ID
  private fetchCardDetails(cardId: string | undefined): void {
    this.apiService.getCardDetails(cardId).subscribe(
      (details) => {
        this.cardDetails = details;
        if (this.cardDetails?.name) {
          this.fetchAlternateVersions(this.cardDetails.name);
        }
      },
      (error) => {
        console.error('Error fetching card details:', error);
      }
    );
  }

  // Fetch alternate versions of the card by name
  private fetchAlternateVersions(cardName: string | undefined): void {
    this.apiService.getCardsByName(cardName).subscribe(
      (versions) => {
        this.alternateVersions = versions || [];
      },
      (error) => {
        console.error('Error fetching alternate versions:', error);
        this.alternateVersions = [];
      }
    );
  }

  // Open the modal
  openModal(selectedCardId: string | undefined, selectedCardName: string | undefined, selectedCardImage?: string | undefined): void {
    this.addCollectIsOpen = true;
    this.cardId = selectedCardId;
    this.cardName = selectedCardName;
    this.cardImage = selectedCardImage;
    this.fetchCardDetails(this.cardId);
    const backdrop = document.createElement('div');
    backdrop.className = 'modal-backdrop fade show';
    document.body.appendChild(backdrop);
    document.body.classList.add('modal-open');
  }

  // Close the modal
  closeModal(): void {
    this.addCollectIsOpen = false;
    const modal = document.getElementById('addToCollectionModal');
    if (modal) {
      modal.classList.remove('show');
      setTimeout(() => modal.style.display = 'none', 150);
    }
    const backdrop = document.querySelector('.modal-backdrop');
    if (backdrop) {
      backdrop.remove();
    }
  }
}
