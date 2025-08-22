import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { User } from '../models/user';
import { catchError, map, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);

  baseUrl = environment.apiUrl

  currentUser = signal<User | null>(null);
  errorMessage: any;

  setCurrentUser(user: User){
    // No longer storing in localStorage - using HTTP-only cookies
    this.currentUser.set(user);
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'Register_Login/login', model, { withCredentials: true }).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  loginWithGoogle() {
    return this.http.get<User>(this.baseUrl + 'Register_Login/google-login', { withCredentials: true }).pipe(
      map(user => {       
          this.currentUser.set(user);
      })
    )
  }

  register(model: any){
    return this.http.post<User>(this.baseUrl + 'Register_Login/register', model, { withCredentials: true }).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
          
        }
        return user;
      }),
      catchError((error) => {
        // Pass the error to the component (you can modify this logic based on the error structure)
        return throwError(() => new Error(error));  // you should see it
      })
    );
  }

  forgotPassword(model: { email: string }) {
    return this.http.post<{ message: string; email: string }>(this.baseUrl + 'Register_Login/ForgotPassword', model);
  }

  getCurrentUser() {
    return this.http.get<User>(this.baseUrl + 'Register_Login/me', { withCredentials: true });
  }

  logout() {
    return this.http.post(this.baseUrl + 'Register_Login/logout', {}, { withCredentials: true }).pipe(
      map(() => {
        this.currentUser.set(null);
      })
    );
  }

 
}
