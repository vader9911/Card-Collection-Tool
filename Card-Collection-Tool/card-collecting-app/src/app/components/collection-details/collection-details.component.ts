import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CollectionsService } from '../../services/collections.service';

@Component({
  selector: 'app-collection-details',
  standalone: true,
  imports: [],
  templateUrl: './collection-details.component.html',
  styleUrl: './collection-details.component.css'
})
export class CollectionDetailsComponent implements OnInit {
  collectionName: string = ''; // Name of the collection
  cards: any[] = []; // List of cards in the collection

  constructor(private route: ActivatedRoute, private collectionsService: CollectionsService) { }

  ngOnInit(): void {
    const collectionId = this.route.snapshot.paramMap.get('id');
    this.loadCollectionDetails(collectionId);
  }

  // Fetch collection details from the server
  loadCollectionDetails(collectionId: number | string | null): void {
    if (!collectionId) {
      return;
    }

    this.collectionsService.getCollectionDetails(collectionId).subscribe(
      (response) => {
        this.collectionName = response.collectionName;
        this.cards = response.cards;
      },
      (error) => {
        console.error('Error fetching collection details:', error);
      }
    );
  }
}
