import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { LoggerService } from '../services/logger.service';

export const AuthGuard: CanActivateFn =
  (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const authService = inject(AuthService);
    const logger = inject(LoggerService);
    const router = inject(Router);
    logger.log(route.url);
    logger.log(route.url[0].path);
    authService.isLoggedIn || router.navigate(['login']);
    return true;
  };
