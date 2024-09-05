import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CollectionsComponent } from './components/collections/collections.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { LoginPartialComponent } from './shared/login-partial/login-partial.component';
import { AuthGuard } from './services/auth.guard.service';
import { CardSearchComponent } from './components/card-search/card-search.component';



export const appRoutes: Routes = [
  {
    path: '',
  component: LayoutComponent, // Main layout with the card search bar
  children: [
    { path: '', component: CardSearchComponent }, // Default child route for the search bar
  ]
  },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'collections', component: CollectionsComponent, canActivate: [AuthGuard] },
  { path: 'privacy', component: PrivacyComponent },
  { path: 'login-partial', component: LoginPartialComponent },
  { path: '**', redirectTo: '' } // Redirect any unknown paths to the home route
];

