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
  isSidebarCollapsed: boolean = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  expandMainContent(): void {
    this.isSidebarCollapsed = true;
  }
}
