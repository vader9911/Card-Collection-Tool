import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, filter } from 'rxjs/operators';
import { ApiService } from '../../services/api.service'; // Import ApiService
import { CardListComponent } from '../card-list/card-list.component'

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
  searchControl = new FormControl(''); // Reactive form control for the search input
  cards: any[] = []; // Holds the search results

  constructor(private ApiService: ApiService) { }

  ngOnInit() {
    // Subscribe to changes in the search input
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300), // Wait 300ms after the last keystroke before considering the input
        distinctUntilChanged(), // Only proceed if the new value is different from the previous value
        filter((query): query is string => query !== null),
        switchMap((query) => this.ApiService.searchCards(query)) // Switch to new observable for each input change
      )
      .subscribe(
        (results) => {
          this.cards = results; // Update the card list with the search results
        },
        (error) => {
          console.error('Error fetching search results:', error);
        }
      );
  }
}
