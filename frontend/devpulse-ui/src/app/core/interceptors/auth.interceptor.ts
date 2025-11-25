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
This interceptor checks if an outgoing HTTP request is targeting a protected backend API.
If so, it decides whether to attach an Entra token (for the initial /me call) or
a DevPulse app token (for all subsequent calls).
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
      (entry) => entry.url && req.url.startsWith(entry.url)
    );

    // ✅ If no match, pass request through unchanged
    if (!matched) {
      return next.handle(req);
    }

    // ✅ Decide which token to attach
    const meApiCall = req.url.endsWith('/me');
    return meApiCall
      ? this.attachEntraToken(req, next, matched.scopes)
      : this.attachDevPulseToken(req, next);
  }

  // 🔹 Method 1: Attach Entra token for /me bootstrap call
  private attachEntraToken(
    req: HttpRequest<any>,
    next: HttpHandler,
    scopes: string[]
  ): Observable<HttpEvent<any>> {
    // ✅ Acquire Entra issued token silently for matched scopes
    const activeAccount = this.msal.instance.getActiveAccount();
    return from(
      this.msal.instance.acquireTokenSilent({
        account: activeAccount ?? undefined,
        scopes,
      })
    ).pipe(
      switchMap((result) => {
        // ✅ Attach Entra issued token to Authorization header
        const authReq = req.clone({
          setHeaders: { Authorization: `Bearer ${result.accessToken}` },
        });
        return next.handle(authReq);
      }),
      catchError((error) => {
        console.error('❌ Entra token acquisition failed:', error);
        return throwError(() => error);
      })
    );
  }

  // 🔹 Method 2: Attach DevPulse app token for all other API calls
  private attachDevPulseToken(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const devPulseJwtToken = localStorage.getItem('devpulse_token');

    if (!devPulseJwtToken) {
      console.warn('⚠️ No DevPulse JWT found in local storage.');
      return next.handle(req); // fallback: send request without token
    }

    // ✅ Attach DevPulse token containing user roles to Authorization header
    const authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${devPulseJwtToken}` },
    });
    return next.handle(authReq);
  }
}
