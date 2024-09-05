import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardSearchComponent } from '../card-search/card-search.component';

@Component({
  selector: 'app-card-list',
  standalone: true,
  imports: [
    CommonModule,
    CardListComponent
  ],
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.scss']
})
export class CardListComponent {
  @Input() cards: any[] = [];
  @Input() searchPerformed: boolean = false;
  @Input() noResultsReturned: boolean = false;
}
