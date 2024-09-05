import { Routes } from '@angular/router';
import { LayoutComponent } from './layout.component';
import { LoginComponent } from './login/login.component';
import { CollectionsComponent } from './collections/collections.component';
import { PrivacyComponent } from './privacy/privacy.component';
import { LoginPartialComponent } from './shared/login-partial/login-partial.component';


export const appRoutes: Routes = [
  { path: '', redirectTo: 'cards', pathMatch: 'full' }, // Default child route
  { path: 'login', component: LoginComponent },
  { path: 'collections', component: CollectionsComponent },
  { path: 'privacy', component: PrivacyComponent },
  { path: 'login-partial', component: LoginPartialComponent },
];

