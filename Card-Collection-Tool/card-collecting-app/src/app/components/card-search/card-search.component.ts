import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, exhaustMap, filter, catchError } from 'rxjs/operators';
import { ApiService } from '../../services/api.service'; // Import ApiService
import { CardListComponent } from '../card-list/card-list.component'
import { Observable, of } from 'rxjs';
import { SearchService } from '../../services/search.service';

@Component({
  selector: 'app-card-search',
  standalone: true,
  imports:
    [
      ReactiveFormsModule,
      CardListComponent,
      CommonModule
  ],
  templateUrl: './card-search.component.html',
  styleUrls: ['./card-search.component.scss'],
  providers: [ApiService]
})
export class CardSearchComponent implements OnInit {
  @Output() searchTermChanged = new EventEmitter<string>(); // Output event to emit search term
  @Output() searchEvent = new EventEmitter<void>();
  searchControl = new FormControl(''); // Reactive form control for the search input
  cards: any[] = []; // Holds the search results
  searchPerformed = false; // Flag to check if a search was performed
  noResultsReturned = false // Flag to check if any results were found
  errorMessage: string = ''; // To display error messages to the user
  constructor(private ApiService: ApiService, private searchService: SearchService) { }

  ngOnInit() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        filter((query): query is string => query !== null),
        exhaustMap((query) =>
          this.ApiService.searchCards(query).pipe(
            catchError(error => {
              if (error.status === 400) { // Check if the error is a 400 Bad Request
                this.resetSearch(); 
              }
              this.errorMessage = 'Search failed. Please try again.';
              console.error('Error fetching search results:', error);
              return of([]); // Return an empty array on error to continue the stream
            })
          )
        )
      )
      .subscribe(
        (results) => {
          this.cards = results; // Update the card list with the search results
          this.searchPerformed = true;
          if (this.cards.length === 0 && this.searchPerformed === true) {
            this.noResultsReturned = true;
          };
          console.log(this.noResultsReturned);
          console.log(this.cards);
        },
        (error) => {
          console.error('Error fetching search results:', error);
          this.cards = []; // Clear results on error
          this.searchPerformed = true;
        }
      );
  }

  resetSearch() {
    this.searchControl.setValue(''); 
    this.cards = [];
    this.searchPerformed = false;
    this.errorMessage = '';
  }

  onSearch(event: Event) {
    const inputElement = event.target as HTMLInputElement;

    this.searchPerformed = true;
    this.searchEvent.emit();

    const term = inputElement.value;
    this.searchTermChanged.emit(term);
    this.searchService.setSearchActive(!!term); // Update the service with the search state
  }

}
