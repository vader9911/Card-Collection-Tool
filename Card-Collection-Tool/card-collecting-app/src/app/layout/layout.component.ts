import { Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginComponent } from '../components/login/login.component';
import { CollectionsComponent } from '../components/collections/collections.component';
import { PrivacyComponent } from '../components/privacy/privacy.component';
import { LoginPartialComponent } from '../shared/login-partial/login-partial.component';
import { CardSearchComponent } from '../components/card-search/card-search.component';
import { CardListComponent } from '../components/card-list/card-list.component';
import { SearchService } from '../services/search.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    RouterModule,
    LoginComponent,
    LoginPartialComponent,
    CollectionsComponent,
    PrivacyComponent,
    CardSearchComponent,
    CardListComponent

  ],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
  isHomePage: boolean = false;
  searchActive: boolean = false;
  showWelcomeSection: boolean = true; // Track visibility of the welcome section

  constructor(private router: Router, private searchService: SearchService) {
    this.router.events.subscribe(() => {
      this.isHomePage = this.router.url === '/';
      if (!this.isHomePage) {
        this.showWelcomeSection = false; // Hide the section if not on the home page
      }
    });
  }

  ngOnInit() {
    // Subscribe to the search state changes from the service
    this.searchService.searchActive$.subscribe((active: boolean) => {
      if (active) {
        this.searchActive = true;
        // Trigger fade out, then hide after a short delay to allow transition to finish
        setTimeout(() => this.showWelcomeSection = false, 500);
      } else {
        // When the search is cleared, ensure the section fades back in smoothly
        this.showWelcomeSection = true; 
        setTimeout(() => this.searchActive = false, 0); 
      }
    });
  }

  handleTransitionEnd() {
    if (this.searchActive) {
      this.showWelcomeSection = false; // Ensure the section is removed after fade-out
    }
  }
}
