import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const landingGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const isLoggedIn = auth.isLoggedIn();
  console.log('landing guard - is logged In: ', isLoggedIn);

  if (isLoggedIn) {
    return router.parseUrl('/dashboard');
  }

  return router.parseUrl('/overview');
};
