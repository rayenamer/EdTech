import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { AdminService } from '../services/admin.service';

export const adminGuard: CanActivateFn = (route, state) => {
  
  let adminService = inject(AdminService);
  const toastr = inject(ToastrService);
  if(adminService.currentUser()){
    return true;
  } else {
    toastr.error('you are not authorized to access this area');
  }

  return true;
};