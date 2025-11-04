/*
To integrate MSAL Angular v4, I bootstrapped DevPulse using AppModule, which requires the root component to be non-standalone. 
I disabled standalone: true and removed the imports array to align with Angular’s module-based bootstrapping rules.
*/

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import {
  HTTP_INTERCEPTORS,
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';

// MSAL Angular v4 and MSAL.js core
import { MsalModule, MsalService, MsalGuard } from '@azure/msal-angular';
import { PublicClientApplication, InteractionType } from '@azure/msal-browser';

// Root component and layout shell
import { App } from './app';
import { routes } from './app.routes';
import { environment } from '../environments/environment';

import { APP_INITIALIZER } from '@angular/core';
import { initializeMsal } from './msal-initializer';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';

@NgModule({
  declarations: [
    App, // ✅ Declare the root component here
  ],

  // ✅ Import core Angular modules and configure routing
  imports: [
    BrowserModule,

    RouterModule,

    // ✅ Define app routes with lazy-loaded standalone components
    RouterModule.forRoot(routes),

    // ✅ Configure MSAL Angular v4 using forRoot (requires NgModule)
    MsalModule.forRoot(
      new PublicClientApplication({
        auth: {
          clientId: environment.msal.clientId, // Replace with actual client ID
          authority: environment.msal.authority,
          redirectUri: environment.msal.redirectUri, // Local dev redirect
        },
        cache: {
          cacheLocation: 'localStorage', // Persist tokens in localStorage
          storeAuthStateInCookie: false, // Optional fallback for older browsers
        },
      }),
      {
        interactionType: InteractionType.Redirect, // Login method
        authRequest: {
          scopes: ['openid', 'profile', 'email'], // Basic identity scopes
        },
      },
      {
        interactionType: InteractionType.Redirect, // Token acquisition method
        protectedResourceMap: new Map([
          // Protect all micro services API calls with scope
          [
            environment.msal.protectedResources.userApi.url,
            environment.msal.protectedResources.userApi.scopes,
          ],
          [
            environment.msal.protectedResources.taskApi.url,
            environment.msal.protectedResources.taskApi.scopes,
          ],

          [
            environment.msal.protectedResources.userApi.url,
            environment.msal.protectedResources.userApi.scopes,
          ],
          [
            environment.msal.protectedResources.taskApi.url,
            environment.msal.protectedResources.taskApi.scopes,
          ],
          [
            environment.msal.protectedResources.moodApi.url,
            environment.msal.protectedResources.moodApi.scopes,
          ],
          [
            environment.msal.protectedResources.journalApi.url,
            environment.msal.protectedResources.journalApi.scopes,
          ],
          [
            environment.msal.protectedResources.dashboardApi.url,
            environment.msal.protectedResources.dashboardApi.scopes,
          ],
          [
            environment.msal.protectedResources.tenantApi.url,
            environment.msal.protectedResources.tenantApi.scopes,
          ],
        ]),
      }
    ),
  ],

  // ✅ Provide MSAL services and modern HttpClient setup
  providers: [
    MsalService, // Core MSAL service
    MsalGuard, // Route protection
    provideHttpClient(withInterceptorsFromDi()), // Angular 20+ HttpClient setup

    {
      provide: APP_INITIALIZER,
      useFactory: initializeMsal,
      deps: [MsalService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor, // Auth Interceptor to attach JWT token as a header
      multi: true,
    },
  ],

  // ✅ Bootstrap the root component (must be non-standalone)
  bootstrap: [App],
})
export class AppModule {}
