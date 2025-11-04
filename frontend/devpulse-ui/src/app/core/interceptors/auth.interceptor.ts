// src/app/core/interceptors/auth.interceptor.ts

import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';
import { Observable, from, throwError } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { MsalService } from '@azure/msal-angular';
import { environment } from '../../../environments/environment';

/*
This interceptor checks if an outgoing HTTP request is targeting a protected backend API, 
and if so, it silently fetches an access token from MSAL and attaches it to the request as a Bearer token for authentication.
*/

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private msal: MsalService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // ✅ Extract all protected resource entries from environment
    const entries = Object.values(environment.msal.protectedResources);

    // ✅ Find matching resource based on request URL
    const matched = entries.find(
      (entry) => entry.url && req.url.startsWith(entry.url) // starts with
    );

    // ✅ If no match, pass request through unchanged, no token attached to avoid too much token exposure
    if (!matched) {
      return next.handle(req);
    }

    // ✅ Acquire token silently for matched scopes
    const activeAccount = this.msal.instance.getActiveAccount();
    return from(
      this.msal.instance.acquireTokenSilent({
        account: activeAccount !== null ? activeAccount : undefined,
        scopes: matched.scopes,
      })
    ).pipe(
      switchMap((result) => {
        // ✅ Attach token to Authorization header
        const authReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${result.accessToken}`,
          },
        });
        return next.handle(authReq);
      }),
      catchError((error) => {
        console.error('❌ Token acquisition failed:', error);
        return throwError(() => error);
      })
    );
  }
}
