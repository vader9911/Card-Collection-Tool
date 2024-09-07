import { Component, OnInit, NgModule } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CollectionsService } from '../../services/collections.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';


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
  defaultImageUrl: string = 'https://archive.org/download/placeholder-image/placeholder-image.jpg'; // Defualt image for no card collections

  constructor(private collectionsService: CollectionsService, private router: Router) { }

  ngOnInit(): void {
    this.loadCollections(); // Load collections when the component initializes
  }

  // Fetch collections from the server
  loadCollections(): void {
    this.collectionsService.getCollections().subscribe(
      (response) => {
        this.collections = response; // Store the fetched collections
      },
      (error) => {
        console.error('Error fetching collections:', error);
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

  // Method to get the first card image URL from the collection
  getFirstCardImage(collection: any): string {
    if (collection.cards && collection.cards.length > 0) {
      // Assuming collection.cards is an array of card objects and each card has an 'imageUrl' property
      return collection.cards[0].imageUrl;
    }
    return this.defaultImageUrl; // Return default image if no cards
  }
}
