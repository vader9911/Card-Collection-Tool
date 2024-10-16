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


  getCardDetailsByIds(cardIds: string[] | undefined): Observable<any[]> {

    return this.http.post<any[]>(`${this.apiUrl}/details`, cardIds);
  }

  private handleError(error: HttpErrorResponse) {
    console.error('API call error:', error);
    return throwError('Something went wrong; please try again later.');
  }

  replaceSymbolsWithSvg(cardText: any, symbols: any): string {
    if (Array.isArray(cardText)) {
      return cardText.map(text => {
        // Ensure each item in the array is a string before applying replace
        if (typeof text === 'string') {
          return this.replaceSymbolsInText(text, symbols);
        }
        return text; 
      }).join(' '); 
    } else if (typeof cardText === 'string') {
      // If cardText is already a string, just replace symbols
      return this.replaceSymbolsInText(cardText, symbols);
    } else {
      console.error('Invalid card text format:', cardText);
      return ''; // Return an empty string if it's neither a string nor an array
    }
  }

  // Helper method to replace symbols in text
  private replaceSymbolsInText(text: string, symbols: any): string {
    Object.keys(symbols).forEach(symbol => {
      const regex = new RegExp(`\\${symbol}`, 'g'); 
      text = text.replace(regex, `<img class="symbol-icon" src="${symbols[symbol]}" alt="${symbol}" />`);
    });
    return text;
  }




}
