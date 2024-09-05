import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { LayoutComponent } from './app/layout/layout.component'; 
import { appRoutes } from './app/app.routes';
import { ReactiveFormsModule } from '@angular/forms';


bootstrapApplication(LayoutComponent, {
  providers: [
    provideRouter(appRoutes),
    provideHttpClient(),
    ReactiveFormsModule
  ]
}).catch(err => console.error(err));
