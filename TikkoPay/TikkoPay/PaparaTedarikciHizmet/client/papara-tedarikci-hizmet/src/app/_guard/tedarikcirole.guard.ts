import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { inject } from '@angular/core';

export const tedarikciRoleGuard: CanActivateFn = (route, state) => {
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);

  const requiredRoles = route.data['requiredRoles'];

  if (authService.isUserInRoles(requiredRoles)) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
