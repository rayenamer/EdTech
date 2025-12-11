import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApplicationServieService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
   private readonly CACHE_KEY = 'user_applications';

  constructor() { }
  //for admin
  getApplications() {
    return this.http.get(`${this.baseUrl}Application/get-all-application-with-all-data`);
  }
  changeApplicationState(ApplicationId: number, state: string) {
    return this.http.put(`${this.baseUrl}Application/change-application-state/${ApplicationId}?NewState=${state}`, {});
  }

  //for users
  AddApplication(application: any, programId: number) {
    return this.http.post(`${this.baseUrl}Application/add-application/${programId}`, application).pipe(
      tap(() => {
        // Clear cache when a new application is added
           sessionStorage.removeItem(this.CACHE_KEY);
      })
    );
  }
  GetApplicationForLoggedInUser() {
    // Check sessionStorage first
    const cached = sessionStorage.getItem(this.CACHE_KEY);
    if (cached) {
      return of(JSON.parse(cached));
    }
    
    // If not cached, fetch from server
    return this.http.get(`${this.baseUrl}Application/get-applications-for-the-logged-in-user`).pipe(
      tap(data => {
        // Store in sessionStorage
        sessionStorage.setItem(this.CACHE_KEY, JSON.stringify(data));
      })
    );
  }
  // Clear cache on logout
  clearCache() {
    sessionStorage.removeItem(this.CACHE_KEY);
  }
}

