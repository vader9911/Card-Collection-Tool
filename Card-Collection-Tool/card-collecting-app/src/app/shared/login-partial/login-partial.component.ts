import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginComponent } from '../../components/login/login.component'
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login-partial',
  standalone: true,
  imports: [
    CommonModule,
    LoginComponent,
    RouterLink,

  ],
  templateUrl: './login-partial.component.html',
  styleUrls: ['./login-partial.component.scss']
})
export class LoginPartialComponent implements OnInit, OnDestroy {
  isAuthenticated: boolean = false; // Track the user's authentication status
  userName: string = ''; // Store the user's name
  private authSubscription!: Subscription;
  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit() {
    // Subscribe to authentication state changes
    this.authSubscription = this.authService.isLoggedIn().subscribe((isLoggedIn) => {
      this.isAuthenticated = isLoggedIn;
      this.userName = isLoggedIn ? this.authService.getUserName() : ''; // Update userName based on login state
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']); // Redirect to the login page after logout
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  ngOnDestroy() {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe(); // Clean up the subscription
    }
  }
}
