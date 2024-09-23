import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'https://localhost:7047/api/cards';

  constructor(private http: HttpClient) { }

  getCardDetails(cardId: string | undefined): Observable<any> {
    console.log('API Service called with card ID:', cardId);
    return this.http.get<any>(`${this.apiUrl}/${cardId}/details`).pipe(
      catchError(this.handleError)
    );
  }

  getCardsByName(cardName: string | undefined): Observable<any> {
    console.log('API Service called with card ID:', cardName);
    return this.http.get<any>(`${this.apiUrl}/${cardName}/variations`).pipe(
      catchError(this.handleError)
    );
  }



  private handleError(error: HttpErrorResponse) {
    console.error('API call error:', error);
    return throwError('Something went wrong; please try again later.');
  }
}
