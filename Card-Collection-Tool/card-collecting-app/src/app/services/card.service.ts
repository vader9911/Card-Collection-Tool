import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environment'; // Correct path to the environment file
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CardService {
  private apiUrl = `${environment.apiUrl}cards`; // Use base URL from environment

  constructor(private http: HttpClient) { }

  getCards(): Observable<any> {
    return this.http.get<any>(this.apiUrl);
  }

  // Other CRUD methods can be added here (create, update, delete)
}
