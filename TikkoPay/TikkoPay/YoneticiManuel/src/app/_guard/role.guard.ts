import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { inject } from '@angular/core';

export const roleGuard: CanActivateFn = (route, state) => {

    // const authService: AuthService = inject(AuthService);
    // const router: Router = inject(Router);

    // const requiredRoles = route.data['requiredRoles'];
    // const requiredAuthorization = route.data['requiredAuthorization'];

    // if (authService.isUserInRoles(requiredRoles) &&
    //     authService.userHasAuthorization(requiredAuthorization)) {
    //     return true;
    // }

    // router.navigate(['/no-auth']);
    // return false;

    return true;
};
