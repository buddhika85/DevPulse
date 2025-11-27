import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable } from 'rxjs';

// Marks this class as injectable so Angular can use it as an HTTP interceptor
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  // Inject Angular Router to perform navigation on error
  private router = inject(Router);

  intercept(
    req: HttpRequest<any>, // the outgoing HTTP request
    next: HttpHandler // the next handler in the chain
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle specific error status codes
        if (
          error.status === 401 || // Unauthorized → shows unauthorized
          error.status === 403 || // Forbidden → show access denied
          error.status === 404 || // Not found → show not found page
          error.status === 500 // Server error → show generic error page
        ) {
          this.router.navigate(['/error-status-code', error.status]);
        } else {
          // For other errors, just log them to console
          console.error(
            `Error ${error.status} - errorInterceptor says `,
            error
          );
        }

        // Re-throw the error so subscribers can still react if needed
        return throwError(() => error);
      })
    );
  }
}
