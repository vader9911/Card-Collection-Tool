import { Component, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { CollectionsService } from '../../services/collections.service';

@Component({
  selector: 'app-collections',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './collections.component.html',
  styleUrl: './collections.component.scss'
})
export class CollectionsComponent implements OnInit {
  collections: any[] = [];

  constructor(private collectionsService: CollectionsService, private authService: AuthService) { }

  ngOnInit(): void {
    this.loadCollections();
  }

  loadCollections() {
    this.collectionsService.getCollections().subscribe((collections) => {
      this.collections = collections;
    });
  }

  addNewCollection(collectionName: string) {
    this.collectionsService.createCollection(collectionName).subscribe((newCollection) => {
      this.collections.push(newCollection);
    });
  }

  viewCollection(collectionId: number) {
    // Logic to view collection details or navigate to the collection detail page
  }
}
