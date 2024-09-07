import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { CollectionsService } from '../../services/collections.service';
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

  constructor(
    private authService: AuthService,
    private collectionsService: CollectionsService,
    private route: ActivatedRoute,
    private router: Router // Inject Router
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

  // Method to delete the collection with confirmation
  confirmDeleteCollection(): void {
    const confirmed = window.confirm('Are you sure you want to delete this collection? This action cannot be undone.');
    if (confirmed) {
      this.collectionsService.deleteCollection(this.collectionId).subscribe(
        () => {
          console.log('Collection deleted successfully.');
          alert('Collection has been deleted.'); // Optional: Alert to notify user
          this.router.navigate(['/collections']); // Redirect to collections list after deletion
        },
        (error) => {
          if (error.status === 401 || error.status === 403) {
            console.error('You are not authorized to delete this collection.');
            alert('You are not authorized to delete this collection.');
          } else if (error.status === 404) {
            console.error('Collection not found.');
            alert('Collection not found.');
          } else {
            console.error('Error deleting collection:', error);
            alert('An unexpected error occurred while deleting the collection.');
          }
        }
      );
    }
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
