import { HttpErrorResponse, HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { HttpEvent, HttpHandler, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';

export const ResponseInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);
  
  return next(req).pipe(
    tap({
      next: (event) => {
        // Check if the response is of type HttpResponse
        if (event instanceof HttpResponse) {
          console.log('Response Body:', event);  // Logs the body of the response
          //const errorMessage = event.error?.message;
          //toastr.error(event);
        }
      },
      error: (err) => {
        if (err instanceof HttpErrorResponse) {
          console.error('Error Response:', err.error);  // Logs the error array or error object from the response
          const errorMessage = err.error;
          toastr.error(errorMessage);
        }
        if (err.error && Array.isArray(err.error) && err.error.length > 0) {
          const errorMessage = err.error[0].description; // Extract error message
          toastr.error(errorMessage);  // Show the toast directly in the interceptor
        } else {
          toastr.error("Error");
        }

        return throwError(() => err.error); 
      }
    })
  );
};
