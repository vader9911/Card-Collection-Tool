import { Component, OnInit, EventEmitter, Output, Input, NgModule, OnDestroy, ElementRef, AfterViewInit, NgZone} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormsModule, FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged, switchMap, filter, catchError, tap } from 'rxjs/operators';
import { Observable, Subscription, of } from 'rxjs';
import { ApiService } from '../../services/api.service';
import { CardSearchRequest, SearchService } from '../../services/search.service';
import { CardListComponent } from '../card-list/card-list.component';
declare var bootstrap: any;
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
  isDrawerOpen: boolean = false; // Replaced modal with drawer
  loading: boolean = false;
  selectedTypes: string[] = [];
  showWarning: boolean = false;
  cardTypes: string[] = [
    'Artifact',
    'Battle',
    'Conspiracy',
    'Creature',
    'Dungeon',
    'Emblem',
    'Enchantment',
    'Hero',
    'Instant',
    'Kindred',
    'Land',
    'Phenomenon',
    'Plane',
    'Planeswalker',
    'Scheme',
    'Sorcery',
    'Vanguard'
  ];


  private searchSubscription: Subscription | null = null;

  constructor(private fb: FormBuilder, private searchService: SearchService, private elementRef: ElementRef, private ngZone: NgZone) {
    this.searchForm = this.fb.group({
      name: [''],
      set: [''],
      oracleText: [''],
      type: [[]], // Update to an array to handle multiple card types
      colors: [[]],
      colorCriteria: ['any'],
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

  ngAfterViewInit(): void {
    this.initializeDropdown();
  }

  // Initialize dropdown functionality in Bootstrap
  initializeDropdown(): void {
    this.ngZone.runOutsideAngular(() => {
      const dropdownElement = this.elementRef.nativeElement.querySelector('#cardTypeDropdown');
      if (dropdownElement) {
        dropdownElement.addEventListener('click', () => {
          const bootstrapDropdown = new bootstrap.Dropdown(dropdownElement);
          bootstrapDropdown.toggle();
        });
      }
    });
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

  onSearch(): void {
    const formValues = this.searchForm.value;
    if ((formValues.name && formValues.name.length < 3) ||
      (formValues.set && formValues.set.length < 3) ||
      (formValues.oracleText && formValues.oracleText.length < 3)) {
      this.showWarning = true;
      setTimeout(() => this.showWarning = false, 3000); // Hide the warning after 3 seconds
      return;
    }

    // Reset warning if search proceeds
    this.showWarning = false;

    const searchParams: CardSearchRequest = {
      name: this.searchForm.value.name || undefined,
      set: this.searchForm.value.set || undefined,
      oracleText: this.searchForm.value.oracleText || undefined,
      type: this.searchForm.value.type.length ? this.searchForm.value.type.join(',') : undefined,
      colors: this.searchForm.value.colors.length ? this.searchForm.value.colors.join(',') : undefined,
      colorCriteria: this.searchForm.value.colorCriteria || 'any',
      colorIdentity: this.searchForm.value.colorIdentity.length ? this.searchForm.value.colorIdentity.join(',') : undefined,
      colorIdentityCriteria: this.searchForm.value.colorIdentityParams || 'any',
      manaValue: this.searchForm.value.manaValue || undefined,
      manaValueComparator: this.searchForm.value.manaValueComparator || 'equals',
      power: this.searchForm.value.power !== null ? this.searchForm.value.power.toString() : undefined,
      powerComparator: this.searchForm.value.powerComparator || 'equals',
      toughness: this.searchForm.value.toughness !== null ? this.searchForm.value.toughness.toString() : undefined,
      toughnessComparator: this.searchForm.value.toughnessComparator || 'equals',
      loyalty: this.searchForm.value.loyalty !== null ? this.searchForm.value.loyalty.toString() : undefined,
      loyaltyComparator: this.searchForm.value.loyaltyComparator || 'equals',
      sortOrder: this.searchForm.value.sortOrder || 'name',
      sortDirection: this.searchForm.value.sortDirection || 'asc'
    };

    console.log('Form Values to be sent:', searchParams);

    this.searchService.searchCards(searchParams).subscribe(cards => {
      this.cards = cards;
      this.noResultsReturned = cards.length === 0;
    }, error => {
      console.error('Error fetching cards:', error);
      this.noResultsReturned = true;
    });
  }



  onTypeSelectionChange(type: string, event: any): void {
    if (event.target.checked) {
      this.selectedTypes.push(type);
    } else {
      const index = this.selectedTypes.indexOf(type);
      if (index > -1) {
        this.selectedTypes.splice(index, 1);
      }
    }
    // Update the form control value to keep it in sync
    this.searchForm.get('type')?.setValue(this.selectedTypes);
  }


  toggleDrawer(): void {
    this.isDrawerOpen = !this.isDrawerOpen;

    if (this.isDrawerOpen) {
      // Copy the name field from the main search to the advanced search form
      const nameValue = this.searchForm.get('name')?.value;
      if (nameValue) {
        this.searchForm.patchValue({ name: nameValue });
      }
    }
  }

  closeDrawer(): void {
    this.isDrawerOpen = false;
  }




 onCheckboxChange(event: any, controlName: string) {
    const formArray: FormArray = this.searchForm.get(controlName) as FormArray;

    if (event.target.checked) {
      // Add the value if checked
      formArray.value.push(event.target.value);
    } else {
      // Remove the value if unchecked
      const index = formArray.value.indexOf(event.target.value);
      if (index !== -1) {
        formArray.value.splice(index, 1);
      }
    }
    this.searchForm.get(controlName)?.setValue([...formArray.value]);
  }

  resetForm(): void {
    this.searchForm.reset({
      set: '',
      oracleText: '',
      type: [],
      colors: [],
      colorCriteria: 'any',
      colorIdentity: [],
      colorIdentityParams: 'any',
      manaValue: null,
      manaValueComparator: 'equals',
      power: '',
      powerComparator: 'equals',
      toughness: '',
      toughnessComparator: 'equals',
      loyalty: '',
      loyaltyComparator: 'equals',
      sortOrder: 'name',
      sortDirection: 'asc',
      showAllVersions: false
    });

    // Resetting the checkboxes manually to ensure UI matches state
    const checkboxes = document.querySelectorAll<HTMLInputElement>('input[type="checkbox"]');
    checkboxes.forEach((checkbox) => {
      checkbox.checked = false;
    });
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
