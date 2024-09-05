import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginComponent } from '../components/login/login.component';
import { CollectionsComponent } from '../components/collections/collections.component';
import { PrivacyComponent } from '../components/privacy/privacy.component';
import { LoginPartialComponent } from '../shared/login-partial/login-partial.component';
import { CardSearchComponent } from '../components/card-search/card-search.component';
import { CardListComponent } from '../components/card-list/card-list.component';

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
export class LayoutComponent {
  constructor() {
    console.log('LayoutComponent loaded');
  }
}
