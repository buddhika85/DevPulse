import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (
        error.status === 401 ||
        error.status === 403 ||
        error.status === 404 ||
        error.status === 500
      )
        router.navigate(['/error-status-code', error.status]);
      else
        console.error(`Error ${error.status} - errorInterceptor says `, error);

      return throwError(() => error);
    })
  );
};
