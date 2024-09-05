import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'; // Import the AuthService

@Injectable({
  providedIn: 'root'
})
export class CollectionsService {
  private baseUrl = 'https://localhost:7047/api/collections'; 

  constructor(private http: HttpClient, private authService: AuthService) { }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json', // Specify the content type as JSON
      'Authorization': `Bearer ${token}` // Add the token to the headers
    });
  }

  getCollections(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl, { headers: this.getAuthHeaders() });
  }

  createCollection(CollectionName: string): Observable<any> {
    const body = JSON.stringify({ name: CollectionName }); // Convert data to JSON
    return this.http.post<any>(this.baseUrl, body, { headers: this.getAuthHeaders() });
  }

  addCardToCollection(id: number, CardId: number): Observable<any> {
    const body = JSON.stringify({ cardId: CardId }); // Convert data to JSON
    return this.http.post<any>(`${this.baseUrl}/${id}/cards`, body, { headers: this.getAuthHeaders() });
  }
}
