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
      'Content-Type': 'application/json', // Specify the content type as JSON
      'Authorization': `Bearer ${token}` // Add the token to the headers
    });
  }

  getCollections(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl, { headers: this.getAuthHeaders() });
  }

  // Fetch card details by ID (new method)
  getCardById(cardId: string): Observable<any> {
    return this.http.get<any>(`/api/cards/${cardId}`);
  }

  // Fetch details of a specific collection
  getCollectionDetails(collectionId: number | string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${collectionId}/details`,{ headers: this.getAuthHeaders() });
  }

  // Method to create a collection
  createCollection(collectionName: string): Observable<any> {
    const userId = this.authService.getUserId(); // Retrieve the user ID from AuthService
    const body = JSON.stringify({
      collectionName: collectionName,
      userId: userId, // Include UserId
      cardIds: [] // Initialize as empty list if there are no cards initially
    });

    return this.http.post<any>(`${this.baseUrl}/create`, body, { headers: this.getAuthHeaders() }).pipe(
      delay(200), // Add a slight delay (200 ms) to give the server time to commit the data
      catchError((error) => {
        console.error('Error creating collection:', error);
        return throwError(error);
      })
    );
  }

  // Method to delete a collection by ID
  deleteCollection(collectionId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${collectionId}`,{ headers: this.getAuthHeaders() });
  }


  addCardToCollection(collectionId: number, CardId: number, Quantity : number): Observable<any> {
    const payload = { CardId, Quantity };
    const token = localStorage.getItem('auth_token');

    console.log('Sending payload to server:', payload); // Log payload for debugging
    console.log('Collection ID in request URL:', collectionId); // Log collectionId for debugging


    return this.http.post<any>(`${this.baseUrl}/${collectionId}/addCard`, payload, {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      }
    });
  }

  

}
