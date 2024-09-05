import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-partial',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './login-partial.component.html',
  styleUrls: ['./login-partial.component.scss']
})
export class LoginPartialComponent {
  constructor(private authService: AuthService) { }

  isLoggedIn(): boolean {
    return this.authService.getAuthStatus(); // Check if the user is logged in
  }

  getUserName(): string {
    // Retrieve the username from your AuthService
    const token = localStorage.getItem('token');
    if (token) {
      // Decode JWT token to get user information
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.sub; // Assume 'sub' contains the username or email
    }
    return '';
  }

  logout() {
    this.authService.logout(); // Call the AuthService to handle logout
  }
}
