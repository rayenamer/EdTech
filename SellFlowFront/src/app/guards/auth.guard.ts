import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = (route, state) => {
  let authService = inject(AuthService);
  const toastr = inject(ToastrService);
  if(authService.currentUser()){
    return true;
  } else {
    toastr.error('you shall not pass');
    return false;
  }

  return true;
};
