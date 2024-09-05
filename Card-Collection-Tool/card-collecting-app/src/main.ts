import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { LayoutComponent } from './app/layout/layout.component'; 
import { appRoutes } from './app/app.routes'; 

bootstrapApplication(LayoutComponent, {
  providers: [
    provideRouter(appRoutes)
  ]
}).catch(err => console.error(err));
