import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {

  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);

  if (authService.loggedIn()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
