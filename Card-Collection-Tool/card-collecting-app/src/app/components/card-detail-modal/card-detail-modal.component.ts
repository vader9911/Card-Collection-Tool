import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { CollectionsService } from '../../services/collections.service';
import { GroupByPipe } from '../../shared/group-by.pipe';
import { TitlecaseKeyPipe } from '../../shared/titlecase-key.pipe';
import { AddToCollectionModalComponent } from '../../components/addcard-modal/addcard-modal.component'
@Component({
  selector: 'app-card-detail-modal',
  standalone: true,
  imports: [CommonModule, GroupByPipe,
    TitlecaseKeyPipe, AddToCollectionModalComponent],
  templateUrl: './card-detail-modal.component.html',
  styleUrls: ['./card-detail-modal.component.scss'],
})
export class CardDetailModalComponent implements OnInit {
  @Input() cardId: string | undefined;
  @Input() cardName: string | undefined;
  @Input() cardImage: string | undefined;
  @Input() isOpen: boolean = false;

  cardDetails: any | undefined;
  alternateVersions: any[] = [];
  symbols: any = {};
  @ViewChild(AddToCollectionModalComponent) addToCollectionModal!: AddToCollectionModalComponent;
  constructor(private apiService: ApiService, private collectionService: CollectionsService) { }

  ngOnInit(): void {
    console.log('Modal open status:', this.isOpen);
    this.loadSymbols();
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
      const backdrop = document.createElement('div');
      backdrop.className = 'modal-backdrop fade show';
      document.body.appendChild(backdrop);

     
      document.body.classList.add('modal-open');
      console.log('Modal opened:', this.isOpen);
    } else {
      console.error('Invalid cardId or cardName provided to openModal');
    }
  }

  // Closes the modal
  closeModal(): void {
    this.isOpen = false;
    const modal = document.getElementById('cardDetailModal');
    const backdrop = document.querySelector('.modal-backdrop');
    if (backdrop) {
      backdrop.remove();
    }
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
          this.replaceSymbolsInCardDetails();
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

  onVersionClick(versionId: string, versionName: string): void {
    console.log('Version clicked:', versionId, versionName);
    this.cardId = versionId;
    this.cardName = versionName;
    this.fetchCardDetails(versionId); // Fetch details for the clicked version
  }

  loadSymbols() {
    console.log("called load symbols in componenet")
    this.collectionService.getSymbols().subscribe(
      (symbolsData) => {
        this.symbols = symbolsData;
        console.log('Loaded Symbols:', this.symbols);  // Optional logging
      },
      (error) => {
        console.error('Error loading symbols:', error);  // Handle error
      }
    );
  }

  replaceSymbolsInCardDetails() {
    if (this.cardDetails && this.symbols) {
      this.cardDetails.manaCost = this.apiService.replaceSymbolsWithSvg(this.cardDetails.manaCost, this.symbols);
      this.cardDetails.oracleText = this.apiService.replaceSymbolsWithSvg(this.cardDetails.oracleText, this.symbols);
      this.cardDetails.colors = this.apiService.replaceSymbolsWithSvg(this.cardDetails.colors, this.symbols);
    }
  }


  openAddToCollectionModal(cardId: string | undefined, cardImage: string | undefined, cardName: string | undefined) {
    console.log("modal for add card opened", cardId, cardImage);
    this.closeModal();
    this.cardId = cardId; // Store the selected card ID
    const modal = document.getElementById('addToCollectionModal');
    if (this.addToCollectionModal) {
      this.addToCollectionModal.openModal(this.cardId, this.cardName);
    }
  }
}
