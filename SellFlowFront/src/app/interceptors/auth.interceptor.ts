import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { AdminService } from '../services/admin.service';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const adminService = inject(AdminService);
  
  // Check for admin token first (stored separately under 'admin')
  const adminUser = adminService.currentUser();
  if (adminUser && adminUser.token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${adminUser.token}`
      }
    });
    return next(req);
  }

  // If no admin token, check for regular user token (Google user)
  const user = authService.currentUser();
  if (user && user.token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${user.token}`
      }
    });
  }
  
  return next(req);
};

