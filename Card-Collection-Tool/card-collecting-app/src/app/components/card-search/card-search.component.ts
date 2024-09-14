import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, filter, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { ApiService } from '../../services/api.service';
import { SearchService } from '../../services/search.service';
import { CardListComponent } from '../card-list/card-list.component';

@Component({
  selector: 'app-card-search',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    CardListComponent,
  ],
  templateUrl: './card-search.component.html',
  styleUrls: ['./card-search.component.scss'],
  providers: [ApiService]
})
export class CardSearchComponent implements OnInit {
  @Output() searchEvent = new EventEmitter<void>();

  searchControl = new FormControl('');
  typeControl = new FormControl('');
  colorControl = new FormControl('');
  oracleTextControl = new FormControl('');

  cards: any[] = [];
  showAllVersions: boolean = false;
  searchPerformed = false;
  noResultsReturned = false;
  errorMessage: string = '';

  constructor(private searchService: SearchService) { }

  ngOnInit() {
    // Handle search results
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(() =>
          this.performSearch().pipe(
            catchError(error => {
              this.errorMessage = 'Search failed. Please try again.';
              console.error('Error fetching search results:', error);
              return of([]);
            })
          )
        )
      )
      .subscribe(
        (results) => {
          this.cards = results;
          this.searchPerformed = true;
          this.noResultsReturned = this.cards.length === 0;
        },
        (error) => {
          console.error('Error fetching search results:', error);
          this.cards = [];
          this.searchPerformed = true;
        }
      );
  }

  // Perform search with filters
  performSearch(): Observable<any> {
    const query = (this.searchControl.value ?? '').trim(); // Ensure query is not null
    const type = this.typeControl.value?.trim() ?? '';
    const oracleText = this.oracleTextControl.value?.trim() ?? '';

    return this.searchService.searchCards(query, this.showAllVersions, type, oracleText);
  }

  toggleShowAllVersions(): void {
    this.showAllVersions = !this.showAllVersions;
    this.performSearch().subscribe(); // Trigger search again to update results
  }
}
