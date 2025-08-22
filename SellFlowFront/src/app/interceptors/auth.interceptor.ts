import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Clone request to include credentials for cookie-based authentication
  // No need for Authorization headers since we're using HTTP-only cookies
  const authReq = req.clone({
    withCredentials: true
  });
  
  return next(authReq);
};

