import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CollectionsService } from '../../services/collections.service';

@Component({
  selector: 'app-collection-overview',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './collection-overview.component.html',
  styleUrl: './collection-overview.component.css'
})
export class CollectionsOverviewComponent implements OnInit {
  collections: any[] = []; // Array to hold collection data

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
    this.router.navigate(['/collections', collectionId]);
  }
}
