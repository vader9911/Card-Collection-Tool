import { Component, OnInit, NgModule } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CollectionsService } from '../../services/collections.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-collections',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './collections.component.html',
  styleUrl: './collections.component.scss'
})
export class CollectionsComponent implements OnInit {
  collections: any[] = []; // Array to hold collection data
  newCollectionName: string = ''; // New collection name
  defaultImageUrl: string = 'https://archive.org/download/placeholder-image/placeholder-image.jpg'; // Default image for no card collections
  cardImages: { [key: string]: string } = {}; // Store card images by collection ID

  constructor(
    private collectionsService: CollectionsService,
    private apiService: ApiService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadCollections(); // Load collections when the component initializes
  }

  // Fetch collections from the server
  loadCollections(): void {
    this.collectionsService.getCollections().subscribe(
      (response) => {
        this.collections = response; // Store the fetched collections
        console.log('Loaded collections:', this.collections);

        // Fetch first card details for each collection after loading collections
        this.collections.forEach((collection) => {
          if (collection.cardIds && collection.cardIds.length > 0) {
            const firstCardId = collection.cardIds[0].cardId; // Get the first card's ID
            this.fetchCardDetails(firstCardId, collection.id); // Fetch card details using the first card ID
          }
        });
      },
      (error) => {
        console.error('Error fetching collections:', error);
      }
    );
  }

  // Method to fetch the first card details by card ID
  fetchCardDetails(cardId: string, collectionId: number): void {
    console.log(`Fetching details for card ID: ${cardId}, collection ID: ${collectionId}`);

    this.apiService.getCardDetails(cardId).subscribe(
      (details) => {
        if (details && details.imageUri) {
          this.cardImages[collectionId] = details.imageUri; // Store the image URI using collection ID as key
          console.log(`Image fetched for card ID: ${cardId} - URI: ${details.imageUri}`);
        } else {
          console.warn(`No image URI found for card ID: ${cardId}`);
          this.cardImages[collectionId] = this.defaultImageUrl; // Use default image if no URI is found
        }
      },
      (error) => {
        console.error(`Error fetching card details for ID: ${cardId}`, error);
        this.cardImages[collectionId] = this.defaultImageUrl; // Use default image if there is an error
      }
    );
  }

  // Navigate to collection details page
  goToCollectionDetails(collectionId: number): void {
    this.router.navigate(['/collections', collectionId, 'details']);
  }

  // Create a new collection
  createCollection(): void {
    if (this.newCollectionName.trim()) {
      this.collectionsService.createCollection(this.newCollectionName).subscribe(
        (response) => {
          this.collections.push(response); // Add the new collection to the list
          this.newCollectionName = ''; // Clear the input field
        },
        (error) => {
          console.error('Error creating collection:', error);
        }
      );
    } else {
      alert('Please enter a collection name.');
    }
  }
}
