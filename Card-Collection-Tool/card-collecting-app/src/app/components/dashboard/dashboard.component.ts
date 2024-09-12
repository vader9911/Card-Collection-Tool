import { Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CollectionsComponent } from '../../components/collections/collections.component';
import { CardSearchComponent } from '../../components/card-search/card-search.component';
import { CardListComponent } from '../../components/card-list/card-list.component';
import { SearchService } from '../../services/search.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CollectionsComponent,
    CardListComponent,
    CardSearchComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  isSidebarCollapsed: boolean = false; // Track whether the sidebar is collapsed
  isSearchExpanded: boolean = false; // Track whether the search view is expanded

  // Toggle the sidebar's collapsed state
  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  // Handle search event to expand the search view
  handleSearch(): void {
    this.isSearchExpanded = true; // Set the state to expand the search view
    this.isSidebarCollapsed = true; // Automatically collapse sidebar when search is expanded
  }

  // Method to reset the view to show both columns
  resetView(): void {
    this.isSearchExpanded = false; // Reset state to collapse the search view
    this.isSidebarCollapsed = false; // Reset sidebar to its expanded state
  }
}
