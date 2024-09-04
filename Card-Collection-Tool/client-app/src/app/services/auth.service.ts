import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Replace with real authentication logic or use libraries like Auth0 or Firebase
  private loggedIn = false;
  private userName: string = 'Guest';

  // Simulate checking if the user is logged in
  isLoggedIn(): boolean {
    // Replace with real logic to check login status
    return this.loggedIn;
  }

  // Simulate getting the user's name
  getUserName(): string {
    // Replace with real logic to get the user's name
    return this.userName;
  }

  // Simulate logging out
  logout(): void {
    // Implement logout logic here (e.g., clear tokens, call backend, etc.)
    this.loggedIn = false;
  }
}
