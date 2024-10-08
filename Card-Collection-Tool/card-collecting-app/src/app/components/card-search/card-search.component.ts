import { Component, OnInit, EventEmitter, Output, Input, NgModule, OnDestroy} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged, switchMap, filter, catchError, tap } from 'rxjs/operators';
import { Observable, Subscription, of } from 'rxjs';
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
    CardListComponent

  ],
  templateUrl: './card-search.component.html',
  styleUrls: ['./card-search.component.scss'],
  providers: [ApiService, SearchService]})
export class CardSearchComponent implements OnInit, OnDestroy {
  searchForm: FormGroup;
  cards: any[] = [];
  searchPerformed = false;
  noResultsReturned = false;
  errorMessage: string = '';
  isModalOpen: boolean = false;
  loading: boolean = false;

  private searchSubscription: Subscription | null = null;

  constructor(private fb: FormBuilder, private searchService: SearchService) {
    this.searchForm = this.fb.group({
      name: [''],
      set: [''],
      oracleText: [''],
      type: [''],
      colors: [[]],
      colorParams: ['any'],
      colorIdentity: [[]],
      colorIdentityParams: ['any'],
      manaValue: [null],
      manaValueComparator: ['equals'],
      power: [''],
      powerComparator: ['equals'],
      toughness: [''],
      toughnessComparator: ['equals'],
      loyalty: [''],
      loyaltyComparator: ['equals'],
      sortOrder: ['name'],
      sortDirection: ['asc'],
      showAllVersions: [false]
    });
  }

  ngOnInit(): void {
    //this.initAutocomplete('set');
    //this.initAutocomplete('type');
    this.setupSortingListeners();
    this.syncNameFields();
  }

  ngOnDestroy(): void {
    // Unsubscribe to prevent memory leaks
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  setupSortingListeners(): void {
    // Listen for changes in sort order and direction
    this.searchForm.get('sortOrder')?.valueChanges.subscribe(() => {
      this.sortCards();
    });

    this.searchForm.get('sortDirection')?.valueChanges.subscribe(() => {
      this.sortCards();
    });
  }

  sortCards(): void {
    const sortOrder = this.searchForm.get('sortOrder')?.value;
    const sortDirection = this.searchForm.get('sortDirection')?.value;

    // Sort cards based on the selected order and direction
    this.cards.sort((a, b) => {
      let comparison = 0;

      switch (sortOrder) {
        case 'name':
          comparison = a.name.localeCompare(b.name);
          break;
        case 'cmc':
          const cmcA = a.cmc ?? 0; // Use fallback to 0 if undefined
          const cmcB = b.cmc ?? 0;
          comparison = cmcA - cmcB;
          break;
        case 'price':
          // Access price from prices.usd and handle empty or missing values
          const priceA = a.prices?.usd ? parseFloat(a.prices.usd) : 0;
          const priceB = b.prices?.usd ? parseFloat(b.prices.usd) : 0;
          comparison = priceA - priceB;
          break;
        case 'toughness':
          const toughnessA = a.toughness ? Number(a.toughness.replace('*', '0')) : 0;
          const toughnessB = b.toughness ? Number(b.toughness.replace('*', '0')) : 0;
          comparison = toughnessA - toughnessB;
          break;
        case 'power':
          const powerA = a.power ? Number(a.power.replace('*', '0')) : 0;
          const powerB = b.power ? Number(b.power.replace('*', '0')) : 0;
          comparison = powerA - powerB;
          break;
        default:
          comparison = 0;
      }

      // Reverse the order if descending
      return sortDirection === 'asc' ? comparison : -comparison;
    });
  }


  //initAutocomplete(field: string): void {
  //  const formControl = this.searchForm.get(field);
  //  if (!formControl) {
  //    console.error(`Form control for field '${field}' not found`);
  //    return;
  //  }

  //  this.autocompleteOptions[field] = formControl.valueChanges.pipe(
  //    debounceTime(800),
  //    distinctUntilChanged(),
  //    switchMap(value => this.searchService.autocomplete(field, value || '')),
  //    catchError(error => {
  //      console.error('Error in autocomplete observable:', error);
  //      return of([]);
  //    })
  //  );
  //}

  //selectOption(option: string): void {
  //  const typeControl = this.searchForm.get('type');
  //  if (typeControl) {
  //    typeControl.setValue(option);
  //  }
  //}

  onSearch(): void {
    // Clear previous results and reset search state before each search
    this.cards = [];
    this.noResultsReturned = false;
    this.errorMessage = '';
    this.loading = true;

    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe(); // Unsubscribe before making a new request
    }

    const nameValue = this.searchForm.get('name')?.value;
    if (nameValue) {
      this.searchForm.patchValue({ name: nameValue });
    }

    const formData = this.searchForm.value;
    this.closeModal();

    // Initiate the search request
    this.searchSubscription = this.searchService.searchCards(formData).subscribe(
      (results) => {
        this.loading = false;
        if (!results || results.length === 0) {
          this.noResultsReturned = true;
        } else {
          this.cards = results;
          this.searchPerformed = true;
          this.sortCards(); // Sort after receiving results
        }
      },
      (error) => {
        this.loading = false;
        console.error('Error fetching search results:', error);
        this.cards = []; // Clear results in case of error
        this.searchPerformed = true;
        this.errorMessage = 'An error occurred while searching. Please try again.';
      }
    );
  }

  

  syncNameFields(): void {
    // Sync changes from the advanced search 'name' field back to the main search
    this.searchForm.get('name')?.valueChanges.subscribe((value) => {
      const mainSearchField = this.searchForm.get('name');
      if (mainSearchField) {
        mainSearchField.setValue(value, { emitEvent: false });
      }
    });
  }

  openModal(): void {
    // Copy the name field from the main search to the advanced search form
    const nameValue = this.searchForm.get('name')?.value;
    if (nameValue) {
      this.searchForm.patchValue({ name: nameValue }); 
    }

    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
  }
}
