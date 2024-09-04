import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component'; // Adjust path if needed
import { appConfig } from './app/app.config'; // Import your app configuration

bootstrapApplication(AppComponent, appConfig)
  .catch(err => console.error(err));
