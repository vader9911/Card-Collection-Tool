import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { CardSearchComponent } from '../components/card-search/card-search.component';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private apiUrl = 'https://localhost:7047/api/cards';

  private searchActiveSource = new BehaviorSubject<boolean>(false); // Initial state is false
  searchActive$ = this.searchActiveSource.asObservable();
 constructor(private http: HttpClient) { }

   //Method to call the search endpoint with filters
  searchCards(formData: any): Observable<any> {
    // Log formData for debugging
    console.log('Search request formData:', formData);

    // Send formData as the body of the POST request
    return this.http.post<any>(`${this.apiUrl}/search`, formData, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    });
  }


  fetchSetNames(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/set-names`);
  }



  autocomplete(field: string, searchTerm: string): Observable<string[]> {
    console.log("Autocomplete method called");

    if (!this.apiUrl) {
      console.error('API URL is not defined');
      return of([]);
    }

   const url = `${this.apiUrl}/autocomplete/${field}?searchTerm=${encodeURIComponent(searchTerm)}`;
    console.log("Generated URL:", url);

    return this.http.get<string[]>(url)
      .pipe(
        catchError(error => {
          console.error('Autocomplete request failed', error);
          return of([]);
        })
      );
  }



}




export interface CardSearchRequest {
  name?: string;
  set?: string;
  oracleText?: string;
  type?: string;
  colors?: string;
  colorCriteria?: string;
  colorIdentity?: string;
  colorIdentityCriteria?: string;
  manaValue?: number;
  manaValueComparator?: string;
  power?: string;
  powerComparator?: string;
  toughness?: string;
  toughnessComparator?: string;
  loyalty?: string;
  loyaltyComparator?: string;
  sortOrder?: string;
  sortDirection?: string;
}


export interface Card {
  id: string;
  name: string;
  manaCost: string;
  typeLine: string;
  oracleText: string;
  power?: string;
  toughness?: string;
  flavorText?: string;
  artist: string;
  collectorNumber: string;
  releaseDate: string;
  rarity: string;
  colors?: string[];
  colorIdentity?: string[];
  usd?: string;
}



