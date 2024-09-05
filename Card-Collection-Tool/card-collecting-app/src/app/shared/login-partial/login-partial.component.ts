import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service'; // Create an Angular service for authentication

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
  constructor(private authService: AuthService, private router: Router) {}

  // Function to check if the user is logged in
  isLoggedIn(): boolean {
    return this.authService.isLoggedIn(); // Replace with your logic
  }

  // Function to get the user's name
  getUserName(): string {
    return this.authService.getUserName(); // Replace with your logic
  }

  // Function to log the user out
  logout(): void {
    this.authService.logout(); // Replace with your logic
    this.router.navigate(['/']); // Redirect to home or another page after logout
  }
}
