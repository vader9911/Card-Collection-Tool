import { Component, OnInit, NgModule } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CollectionsService } from '../../services/collections.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
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
          this.fetchCollectionCards(collection.collectionID);
          if (collection.cards && collection.cards.length > 0) {
            const firstCardId = collection.cards[0].cardID; // Get the first card's ID
            this.fetchCardDetails(firstCardId, collection.collectionID); // Fetch card details using the correct `collectionID`
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

  fetchCollectionCards(collectionID: number) {  // Ensure `collectionID` is used here
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

  // Example: Calling the update method from the component
  // Example: Calling the update method from the component
  //updateCollectionDetails(): void {
  //  if (this.collection.colle && this.collectionDetails) {
  //    this.collectionsService.updateCollection(
  //      this.collectionId,
  //      this.collectionDetails.collectionName,  // Make sure this matches the correct property name
  //      this.collectionDetails.imageUri,
  //      this.collectionDetails.notes
  //    ).subscribe(
  //      (response) => {
  //        console.log('Collection updated successfully:', response);
  //        // Optionally reload collection data
  //        this.loadCollectionDetails();
  //      },
  //      (error) => {
  //        console.error('Error updating collection:', error);
  //      }
  //    );
  //  }
  //}





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
