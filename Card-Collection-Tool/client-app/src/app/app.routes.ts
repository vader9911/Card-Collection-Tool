import { Routes } from '@angular/router';
import { IndexComponent } from './index/index.component';
import { LoginComponent } from './login/login.component';
import { CollectionsComponent } from './collections/collections.component';
import { PrivacyComponent } from './privacy/privacy.component';
import { LoginPartialComponent } from './shared/login-partial/login-partial.component';

export const routes: Routes = [
  { path: '', component: IndexComponent },
  { path: 'login', component: LoginComponent },
  { path: 'collections', component: CollectionsComponent },
  { path: 'privacy', component: PrivacyComponent },
  { path: 'login-partial', component: LoginPartialComponent },
];
