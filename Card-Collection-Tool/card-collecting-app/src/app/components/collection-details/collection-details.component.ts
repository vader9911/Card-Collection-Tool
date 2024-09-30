import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { CollectionsService } from '../../services/collections.service';

import * as bootstrap from 'bootstrap';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-collection-details',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './collection-details.component.html',
  styleUrls: ['./collection-details.component.scss']
})
export class CollectionDetailsComponent implements OnInit {
  isLoggedIn: boolean = false;
  collectionDetails: any = null;
  authSubscription?: Subscription;
  routeSubscription?: Subscription;
  collectionId: number = 0;
  displayFormat: string = 'grid'; // Default display format is 'grid'
  deleteModal: any;

  constructor(
    private authService: AuthService,
    private collectionsService: CollectionsService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap.subscribe(params => {
      this.collectionId = Number(params.get('collectionId'));
      console.log('Collection ID from route:', this.collectionId);

      if (isNaN(this.collectionId) || this.collectionId <= 0) {
        console.error('Invalid collection ID:', this.collectionId);
        this.router.navigate(['/']); // Redirect to default route if invalid
        return;
      }

      this.authSubscription = this.authService.isLoggedIn().subscribe((status) => {
        this.isLoggedIn = status;
        if (this.isLoggedIn) {
          this.loadCollectionDetails();
        } else {
          this.router.navigate(['/login']); // Redirect to login if not authenticated
        }
      });
    });
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }

  // Method to open the delete confirmation modal
  openDeleteModal(): void {
    const deleteModalElement = document.getElementById('deleteModal');
    if (deleteModalElement) {
      this.deleteModal = new bootstrap.Modal(deleteModalElement);
      this.deleteModal.show();
    }
  }

  // Method to delete the collection
  deleteCollection(): void {
    // Hide the modal explicitly before deletion
    if (this.deleteModal) {
      this.deleteModal.hide();
    }

    this.collectionsService.deleteCollection(this.collectionId).subscribe(
      () => {
        console.log('Collection deleted successfully.');
        this.router.navigate(['/collections']); // Redirect to collections list after deletion
      },
      (error) => {
        console.error('Error deleting collection:', error);
        alert('An unexpected error occurred while deleting the collection.');
      }
    );
  }

  loadCollectionDetails(): void {
    if (this.collectionId) {
      this.collectionsService.getCollectionDetails(this.collectionId).subscribe(
        (details) => {
          this.collectionDetails = details;
          console.log('Collection details loaded:', this.collectionDetails);
        },
        (error) => {
          console.error('Error loading collection details:', error);
        }
      );
    } else {
      console.error('Invalid collection ID');
    }
  }

  // Method to navigate to card details page
  viewCardDetails(cardId: string): void {
    this.router.navigate(['/cards', cardId, 'details']);
  }

  // Method to toggle the display format
  toggleDisplayFormat(format: string): void {
    this.displayFormat = format;
  }
}
