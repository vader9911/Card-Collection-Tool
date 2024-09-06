import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { CollectionsService } from '../../services/collections.service';
import { AddToCollectionModalComponent } from '../addcard-modal/addcard-modal.component'

@Component({
  selector: 'app-card-details',
  standalone: true,
  imports: [
    CommonModule,
    AddToCollectionModalComponent

  ],
  templateUrl: './card-details.component.html',
  styleUrls: ['./card-details.component.scss']
})
export class CardDetailsComponent implements OnInit {
  cardId: string = '';
  cardDetails: any = null;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private collectionsService: CollectionsService
  ) { }

  ngOnInit(): void {
    // Get card ID from the route parameters
    this.cardId = this.route.snapshot.paramMap.get('cardId') ?? '';

    // Load card details if card ID is available
    if (this.cardId) {
      this.apiService.getCardDetails(this.cardId).subscribe(
        (details) => {
          this.cardDetails = details;
        },
        (error) => {
          console.error('Error fetching card details:', error);
        }
      );
    }
  }

  // Function to open the add-to-collection modal
  openAddToCollectionModal(): void {
    // Implement your modal logic here or trigger an existing modal
  }
}
