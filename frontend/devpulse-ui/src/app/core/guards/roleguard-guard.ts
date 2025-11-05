import { CanActivateFn, Router } from '@angular/router';
import { UserRole } from '../models/user-role.enum';
import { AuthService } from '../services/auth';
import { inject } from '@angular/core';
import { UserStoreService } from '../services/user-store.service';

// returns true - if msal cached user available (from entra) and backend retrieved app stored user is within expected roles defined in the Route data
export const roleguardGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const userStoreService = inject(UserStoreService);
  const router = inject(Router);

  const hydrated = userStoreService.isHydrated();

  if (!hydrated) return false; // or redirect to a loading screen

  const expectedRoles = route.data['roles'] as UserRole[];
  const user = authService.getUser(); // MSAL cached user
  const userDto = userStoreService.userDto(); // App Stored user from backend

  if (user && userDto && expectedRoles.includes(userDto.userRole)) {
    // if MSAL user available AND app stored user available AND app users role is what is expected by route role defined
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
