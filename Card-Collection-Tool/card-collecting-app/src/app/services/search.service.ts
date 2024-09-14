import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private apiUrl = 'https://localhost:7047/api/cards';

  private searchActiveSource = new BehaviorSubject<boolean>(false); // Initial state is false
  searchActive$ = this.searchActiveSource.asObservable();

  constructor(private http: HttpClient) { }

  // Method to call the search endpoint with filters
  searchCards(query: string, showAllVersions: boolean, type?: string, oracleText?: string): Observable<any> {
    let params = new HttpParams()
      .set('query', query)
      .set('showAllVersions', showAllVersions.toString());

    // Add filters if they are provided
    if (type) {
      params = params.set('type', type);
    }
    if (oracleText) {
      params = params.set('oracleText', oracleText);
    }

    return this.http.get<any>(`${this.apiUrl}/search`, { params });
  }

  // Method to update the search state
  setSearchActive(active: boolean): void {
    this.searchActiveSource.next(active);
  }
}
