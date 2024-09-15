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
  searchCards(formData: any): Observable<any> {
    // Prepare HttpParams
    let params = new HttpParams();

    // Add all form fields to the parameters if they are provided
    Object.keys(formData).forEach(key => {
      if (formData[key] !== null && formData[key] !== '') {
        params = params.set(key, formData[key].toString());
      }
    });

    return this.http.get<any>(`${this.apiUrl}/search`, { params });
  }
}
