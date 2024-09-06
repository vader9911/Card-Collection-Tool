import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'https://localhost:7047/api/cards';

  constructor(private http: HttpClient) { }

  // Method to call the autocomplete endpoint
  searchCards(query: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/autocomplete?query=${query}`);
  }

  // Method to fetch card details by ID
  getCardDetails(cardId: string): Observable<any> {
    console.log(cardId);
    return this.http.get<any>(`${this.apiUrl}/${cardId}/details`);
  }
}
