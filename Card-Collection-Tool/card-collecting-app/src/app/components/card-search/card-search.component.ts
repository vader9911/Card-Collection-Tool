import { Component } from '@angular/core';
import { ApiService } from '../services/api.service'; // Import the service

@Component({
  selector: 'app-card-search',
  standalone: true,
  templateUrl: './card-search.component.html',
  styleUrls: ['./card-search.component.scss'],
  providers: [ApiService] // Provide the service if needed
})
export class CardSearchComponent {
  searchQuery: string = ''; // Holds the user search input
  cards: any[] = []; // Holds the search results

  constructor(private apiService: ApiService) { }

  onSearch() {
    if (this.searchQuery) {
      this.apiService.searchCards(this.searchQuery).subscribe(
        (results) => {
          this.cards = results; // Assign the results to the cards array
        },
        (error) => {
          console.error('Error fetching search results:', error);
        }
      );
    }
  }
}
