import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, delay, throwError } from 'rxjs';
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
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getCollections(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl, { headers: this.getAuthHeaders() });
  }

  getCardById(cardId: string | undefined): Observable<any> {
    return this.http.get<any>(`/api/cards/${cardId}`);
  }

  getCollectionDetails(collectionId: number | string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${collectionId}/details`, { headers: this.getAuthHeaders() });
  }

  // Updated method to create a collection with additional fields
  createCollection(collectionName: string): Observable<any> {
    const userId = this.authService.getUserId(); // Retrieve the user ID from AuthService
    const defaultImageUri = 'path/to/placeholder-image.png'; // Default image path or URL
    const notes = ''; // Default notes as empty

    const body = JSON.stringify({
      collectionName: collectionName,
      userId: userId,
      imageUri: defaultImageUri, // Set the default image URI
      notes: notes // Set notes to be empty initially
    });

    return this.http.post<any>(`${this.baseUrl}/create`, body, { headers: this.getAuthHeaders() }).pipe(
      delay(200), // Add a slight delay to give the server time to commit the data
      catchError((error) => {
        console.error('Error creating collection:', error);
        return throwError(error);
      })
    );
  }

  // New method to update an existing collection
  updateCollection(collectionId: number, collectionName: string, imageUri: string, notes: string): Observable<any> {
    const userId = this.authService.getUserId();
    const body = JSON.stringify({
      collectionName: collectionName,
      userId: userId,
      imageUri: imageUri,
      notes: notes
    });

    return this.http.put<any>(`${this.baseUrl}/${collectionId}/update`, body, { headers: this.getAuthHeaders() }).pipe(
      delay(200),
      catchError((error) => {
        console.error('Error updating collection:', error);
        return throwError(error);
      })
    );
  }

  deleteCollection(collectionId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${collectionId}`, { headers: this.getAuthHeaders() });
  }

  // Updated method to add a card to a collection and ensure collection summary is updated
  addCardToCollection(collectionId: number, cardId: string, quantity: number): Observable<any> {
    const payload = { collectionID: collectionId, cardID: cardId, quantity: quantity };
    const url = `${this.baseUrl}/upsert-card`;

    console.log(`Making request to: ${url}`);
    console.log('Payload:', payload);

    return this.http.post<any>(url, payload, { headers: this.getAuthHeaders() }).pipe(
      catchError((error) => {
        console.error('Error in addCardToCollection service:', error.message);
        console.log('Error details:', error); // Log the full error details
        return throwError(error);
      })
    );
  }



  // New method to fetch card details and update the collection summary
  getCardIdsByCollectionId(collectionId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/${collectionId}/card-ids`, { headers: this.getAuthHeaders() });
  }

}
