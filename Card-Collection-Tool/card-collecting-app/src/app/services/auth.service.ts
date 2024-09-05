import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7047/api/auth'; // Replace with your actual backend URL
  private isAuthenticated = false; // Track user authentication status

  constructor(private http: HttpClient) { }

  register(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/register`, { email, password });
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/login`, { email, password }).pipe(
      tap(response => {
        localStorage.setItem('token', response.token); // Store token in local storage
        this.isAuthenticated = true;
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token'); // Remove token from local storage
    this.isAuthenticated = false;
  }

  getAuthStatus(): boolean {
    return this.isAuthenticated;
  }
}
