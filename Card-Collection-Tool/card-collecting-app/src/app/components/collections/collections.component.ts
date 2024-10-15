import { Component, OnInit, NgModule } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CollectionsService } from '../../services/collections.service';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup  } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Card } from '../../models/card';
import { Collection } from '../../models/collection';

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
  collections: Collection[] = []; // Array to hold collection data
  newCollectionName: string = ''; // New collection name
  defaultImageUrl: string = 'https://archive.org/download/placeholder-image/placeholder-image.jpg'; // Default image for no card collections
  cardImages: { [key: string]: string } = {}; // Store card images by collection ID
  cardDetailsList: any[] = [];
  sortOption: string = 'name';
  sortDirection: string = 'asc'; // Default direction is ascending
  sortForm: FormGroup;

  constructor(
    private collectionsService: CollectionsService,
    private apiService: ApiService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.sortForm = this.fb.group({
      sortOrder: ['name'], // Default sort order
      sortDirection: ['asc'], // Default sort direction
    });

    // Listen for value changes on the form
    this.sortForm.valueChanges.subscribe(() => {
      this.sortCollections();
    });
}

  ngOnInit(): void {
    this.loadCollections(); // Load collections when the component initializes
  }

  // Fetch collections from the server
  loadCollections(): void {
    this.collectionsService.getCollections().subscribe(
      (response) => {
        this.collections = response; // Store the fetched collections
        console.log('Loaded collections:', this.collections);

        // Loop through each collection and fetch the image or card details
        this.collections.forEach((collection) => {
          this.fetchCardDetailsOrUseCollectionImage(collection);
        });
      },
      (error) => {
        console.error('Error fetching collections:', error);
      }
    );
  }


  sortCollections(): void {
    const { sortOrder, sortDirection } = this.sortForm.value;

    const multiplier = sortDirection === 'asc' ? 1 : -1;

    switch (sortOrder) {
      case 'name':
        this.collections.sort((a, b) => a.collectionName.localeCompare(b.collectionName) * multiplier);
        break;
      case 'totalValue':
        this.collections.sort((a, b) => {
          const valueA = parseFloat(a.totalValue.toString());
          const valueB = parseFloat(b.totalValue.toString());
          console.log('Comparing values:', valueA, valueB);
          return (valueA - valueB) * multiplier;
        });
        break;
      case 'totalCards':
        this.collections.sort((a, b) => (a.totalCards - b.totalCards) * multiplier);
        break;
      default:
        console.warn('Invalid sort option');
    }


    console.log('Sorted Collections:', this.collections);
  }


  // Method to fetch the first card details or use the collection image if available
  fetchCardDetailsOrUseCollectionImage(collection: Collection): void {
    if (collection.imageUri) {
      // If the collection has a custom image, use it
      this.cardImages[collection.collectionID] = collection.imageUri;
      console.log(`Using collection image for collection ID: ${collection.collectionID}`);
    } else if (collection.cards && collection.cards.length > 0) {
      // Otherwise, use the first card's image
      const firstCardId = collection.cards[0].cardID;
      this.fetchCardDetails(firstCardId, collection.collectionID);
    } else {
      // If there are no cards, use a default image
      this.cardImages[collection.collectionID] = this.defaultImageUrl;
    }
  }



  // Method to fetch the first card details by card ID
  // Fetch the card details by card ID and include collection name and ID
  fetchCardDetails(cardId: string, collectionId: number): void {
    console.log(`Fetching details for card ID: ${cardId}, collection ID: ${collectionId}`);

    this.apiService.getCardDetails(cardId).subscribe(
      (details) => {
        if (details && details.imageUri) {
          this.cardImages[collectionId] = details.imageUri; // Store the image URI
          console.log(`Image fetched for card ID: ${cardId} - URI: ${details.imageUri}`);

          // Extend the card details to include the collection name and ID
          const collection = this.collections.find(c => c.collectionID === collectionId);
          if (collection) {
            details.collectionName = collection.collectionName;
            details.collectionID = collection.collectionID;
            console.log('Updated card details with collection info:', details);
          }

          // Push the updated details into the card details list
          this.cardDetailsList.push(details);
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


  fetchCollectionCards(collectionID: number) { 
  this.collectionsService.getCardIdsByCollectionId(collectionID).subscribe(
    (cardIds: string[] | undefined) => {
      console.log('Card IDs retrieved:',cardIds, "For Collection:", collectionID);

      // Use the retrieved card IDs to fetch card details
      this.apiService.getCardDetailsByIds(cardIds).subscribe(
        (cardDetails) => {
          console.log('Card details:', cardDetails);
          this.cardDetailsList = cardDetails; // Store the card details in a list to display
        },
        (error) => {
          console.error('Error fetching card details:', error);
        }
      );
    },
    (error) => {
      console.error('Error fetching card IDs:', error);
    }
  );
  }

  // Navigate to collection details page
  goToCollectionDetails(collectionId: number): void {
    console.log(collectionId);
    this.router.navigate(['/collections', collectionId, 'details']);
  }

  // Create a new collection
  createCollection(): void {
    if (this.newCollectionName.trim()) {
      this.collectionsService.createCollection(this.newCollectionName).subscribe(
        (response) => {
          this.collections.push(response); // Add the new collection to the list
          this.newCollectionName = ''; // Clear the input field
          this.loadCollections();
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
