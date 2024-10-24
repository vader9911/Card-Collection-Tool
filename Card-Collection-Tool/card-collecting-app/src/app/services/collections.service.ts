import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, delay, switchMap, throwError } from 'rxjs';
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
    console.log(HttpHeaders);
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
    const defaultImageUri = ''; // Default image path or URL
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

  //deleteCollection(collectionId: number): Observable<void> {
  //  const headers = new HttpHeaders().set('Authorization', `Bearer ${this.authService.getToken()}`);
  //  return this.http.delete<void>(`${this.baseUrl}/${collectionId}`, { headers: headers });
  //}

  deleteCollection(collectionId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${collectionId}/delete`, { headers: this.getAuthHeaders() });
  }

  // Updated method to add a card to a collection and ensure collection summary is updated
  addCardToCollection(collectionId: number, cardId: string, quantity: number, cardImage: string | undefined): Observable<any> {
    const payload = { collectionID: collectionId, cardID: cardId, quantity: quantity };
    const url = `${this.baseUrl}/upsert-card`;

    // Fetch the collection details first to check if it has an image
    return this.getCollectionDetails(collectionId).pipe(
      switchMap((collectionDetails) => {
        // Check if the collection already has an image
        if (!collectionDetails.imageUri && cardImage) {
          // If there's no image and we have a card image, update the collection with the card's image
          return this.updateCollection(collectionId, collectionDetails.collectionName, cardImage, collectionDetails.notes).pipe(
            switchMap(() => {
              // After updating the collection, proceed to add the card
              return this.http.post<any>(url, payload, { headers: this.getAuthHeaders() });
            })
          );
        } else {
          // If the collection already has an image, simply add the card
          return this.http.post<any>(url, payload, { headers: this.getAuthHeaders() });
        }
      }),
      catchError((error) => {
        console.error('Error in addCardToCollection service:', error.message);
        console.log('Error details:', error);
        return throwError(error);
      })
    );
  }


  getSymbols(): Observable<any> {
    console.log("ran get symbols collection.service.ts")
    const url = `${this.baseUrl}/symbols`; // Ensure this matches your backend route
    return this.http.get<any>(url, { headers: this.getAuthHeaders() });
  }

  // New method to fetch card details and update the collection summary
  getCardIdsByCollectionId(collectionId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/${collectionId}/card-ids`, { headers: this.getAuthHeaders() });
  }

  removeCardFromCollection(collectionId: number, cardId: string | undefined): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.authService.getToken()}`);
    return this.http.delete(`${this.baseUrl}/delete-card`, {
      headers: headers,
      body: { collectionId, cardId }
    });
  }




  updateCardQuantity(collectionId: number, cardId: string | undefined, quantityChange: number): Observable<any> {
    
    return this.http.post(`${this.baseUrl}/update-card-quantity`, { collectionId, cardId, quantityChange }, { headers: this.getAuthHeaders() });
  }

}
