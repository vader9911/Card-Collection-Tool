import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-card-list',
  standalone: true,
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.scss']
})
export class CardListComponent {
  @Input() cards: any[] = [];
}
