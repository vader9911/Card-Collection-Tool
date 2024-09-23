import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-card-detail-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card-detail-modal.component.html',
  styleUrls: ['./card-detail-modal.component.css'],
})
export class CardDetailModalComponent implements OnInit {
  @Input() cardId: string | null = null;
  @Input() cardName: string | null = null;
  @Input() isOpen: boolean = false;

  cardDetails: any;
  alternateVersions: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    console.log('Modal open status:', this.isOpen);

    // Ensure that the modal only fetches data when valid cardId and cardName are passed
    if (this.cardId && this.cardName) {
      this.fetchCardDetails(this.cardId);
      this.fetchAlternateVersions(this.cardName);
    }
  }

  // Opens the modal and fetches the card details
  openModal(cardId: string | undefined, cardName: string | undefined): void {
    this.isOpen = true;
    if (cardId && cardName) {
      this.fetchCardDetails(cardId);
      this.fetchAlternateVersions(cardName);
      console.log('Modal opened:', this.isOpen);
    } else {
      console.error('Invalid cardId or cardName provided to openModal');
    }
  }

  // Closes the modal
  closeModal(): void {
    this.isOpen = false;
    const modal = document.getElementById('cardDetailModal');
    if (modal) {
      modal.style.display = 'none';
    }
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
}
