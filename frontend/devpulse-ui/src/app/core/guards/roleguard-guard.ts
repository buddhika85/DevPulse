import { CanActivateFn, Router } from '@angular/router';
import { UserRole } from '../models/user-role.enum';
import { AuthService } from '../services/auth';
import { inject } from '@angular/core';
import { UserStoreService } from '../services/user-store.service';

// returns true - if msal cached user available (from entra) and backend retrieved app stored user is within expected roles defined in the Route data
export const roleguardGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const userService = inject(UserStoreService);
  const router = inject(Router);

  const expectedRoles = route.data['roles'] as UserRole[];
  const user = authService.getUser(); // MSAL cached user
  const userDto = userService.userDto(); // App Stored user from backend

  if (user && userDto && expectedRoles.includes(userDto.userRole)) {
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
