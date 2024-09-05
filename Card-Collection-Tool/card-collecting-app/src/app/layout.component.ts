import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { CollectionsComponent } from './collections/collections.component';
import { PrivacyComponent } from './privacy/privacy.component';
import { LoginPartialComponent } from './shared/login-partial/login-partial.component';
import { CardSearchComponent } from './card-search/card-search.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    LoginComponent,
    LoginPartialComponent,
    CollectionsComponent,
    PrivacyComponent,
    CardSearchComponent

  ],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {
  constructor() {
    console.log('LayoutComponent loaded');
  }
}
