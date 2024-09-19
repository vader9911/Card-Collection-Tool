import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
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

  // Method to call the search endpoint with filters
  searchCards(formData: any): Observable<any> {
    let params = new HttpParams();

    // Iterate over formData and add each parameter to HttpParams
    Object.keys(formData).forEach(key => {
      if (formData[key] !== null && formData[key] !== '') {
        params = params.set(key, formData[key].toString());
      }
    });

    console.log('Search request parameters:', params.toString()); // Log parameters for debugging

    return this.http.get<any>(`${this.apiUrl}/search`, { params })
      .pipe(
        catchError(error => {
          console.error('Search request failed:', error);
          return of([]); // Handle errors gracefully
        })
      );
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


