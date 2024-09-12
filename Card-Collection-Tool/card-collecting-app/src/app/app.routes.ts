import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CollectionsComponent } from './components/collections/collections.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { LoginPartialComponent } from './shared/login-partial/login-partial.component';
import { AuthGuard } from './services/auth.guard.service';
import { CardSearchComponent } from './components/card-search/card-search.component';
import { CollectionsOverviewComponent } from './components/collection-overview/collection-overview.component';
import { CollectionDetailsComponent } from './components/collection-details/collection-details.component';
import { CardDetailsComponent } from './components/card-details/card-details.component';
import { DashboardComponent } from '../app/components/dashboard/dashboard.component' 

export const appRoutes: Routes = [
  { path: '', component: CardSearchComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'collections', component: CollectionsComponent, canActivate: [AuthGuard] },
  { path: 'privacy', component: PrivacyComponent },
  { path: 'login-partial', component: LoginPartialComponent },
  { path: 'collections', component: CollectionsOverviewComponent },
  { path: 'collections/:collectionId/details', component: CollectionDetailsComponent },
  { path: 'cards/:cardId/details', component: CardDetailsComponent },
  { path: '**', redirectTo: '' } // Redirect any unknown paths to the home route
];

