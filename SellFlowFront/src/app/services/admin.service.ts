import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { User } from '../models/user';
import { catchError, map, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http = inject(HttpClient);

  baseUrl = environment.apiUrl

  currentUser = signal<User | null>(null);
  isAuthenticated = signal<boolean>(false);
  errorMessage: any;

  setCurrentUser(user: User){
    // No longer storing in localStorage - using HTTP-only cookies
    this.currentUser.set(user);
    this.isAuthenticated.set(true);
  }

  setAuthenticated(status: boolean) {
    this.isAuthenticated.set(status);
    if (!status) {
      this.currentUser.set(null);
    }
  }
  

  login(model: any) {
    return this.http.post(this.baseUrl + 'AdminAndModerators/login', model, { withCredentials: true }).pipe(
      map((response) => {
        this.setAuthenticated(true);
        return response;
      })
    );
  }

  register(model: any){
    return this.http.post<User>(this.baseUrl + 'AdminAndModerators/register-admin', model, { withCredentials: true }).pipe(
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


  GetAllUsersForAdmin(){
    return this.http.get<any[]>(this.baseUrl + 'AdminAndModerators/GetAllUsersForAdmin', { withCredentials: true });
  }

  ////getCurrentUser() {
  //  return this.http.get<User>(this.baseUrl + 'AdminAndModerators/me', { withCredentials: true });
  //}

  logout() {
    return this.http.post(this.baseUrl + 'AdminAndModerators/logout', {}, { withCredentials: true }).pipe(
      map(() => {
        this.setAuthenticated(false);
      })
    );
  }

  


  constructor() { }
}
