import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormsModule, FormGroup } from '@angular/forms';
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
  searchForm: FormGroup;
  cards: any[] = [];
  searchPerformed = false;
  noResultsReturned = false;
  errorMessage: string = '';
  isModalOpen: boolean = false;

  constructor(private searchService: SearchService) {
    this.searchForm = new FormGroup({
      name: new FormControl(),
      set: new FormControl(),
      oracleText: new FormControl(),
      type: new FormControl(),
      colors: new FormControl([]),
      colorParams: new FormControl('any'),
      colorIdentity: new FormControl([]),
      colorIdentityParams: new FormControl('any'),
      manaValue: new FormControl(),
      manaValueComparator: new FormControl('equals'), // Comparator for mana value
      manaCost: new FormControl(),
      power: new FormControl(),
      powerComparator: new FormControl('equals'), // Comparator for power
      toughness: new FormControl(),
      toughnessComparator: new FormControl('equals'), // Comparator for toughness
      loyalty: new FormControl(),
      loyaltyComparator: new FormControl('equals'), // Comparator for loyalty
      sortOrder: new FormControl('name'),
      sortDirection: new FormControl('asc'),
      showAllVersions: new FormControl(false)
    });
  }

  ngOnInit() { }

  openModal(): void {
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
  }

  onSearch(): void {
    const formData = this.searchForm.value;
    this.searchService.searchCards(formData).subscribe(
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
}
