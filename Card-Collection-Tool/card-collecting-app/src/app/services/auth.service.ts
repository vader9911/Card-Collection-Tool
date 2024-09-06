import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7047/api/auth';
  private tokenKey = 'auth_token'; // Key to store the token in localStorage
  private authState = new BehaviorSubject<boolean>(this.isTokenValid(this.getToken() || '')); // Initial state based on token validity

  constructor(private http: HttpClient) { }

  register(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/register`, { email, password }).pipe(
      tap(response => this.setToken(response.token))
    );
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/login`, { email, password }).pipe(
      tap(response => this.setToken(response.token))
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.authState.next(false); // Update authentication state to logged out
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
    this.authState.next(true); // Update authentication state to logged in
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): Observable<boolean> {
    return this.authState.asObservable(); // Return authentication state as an observable
  }

  getUserId(): string | null {
    const token = this.getToken();
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1])); // Decode the JWT payload
      return payload['sub']; // Assuming the user ID is stored in the 'sub' claim
    }
    return null;
  }

  getUserName(): string {
    const token = this.getToken();
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.sub;
    }
    return '';
  }

  private isTokenValid(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp;
      const now = Math.floor(new Date().getTime() / 1000);
      return expiry > now;
    } catch (e) {
      console.error('Invalid token:', e);
      return false;
    }
  }
}
