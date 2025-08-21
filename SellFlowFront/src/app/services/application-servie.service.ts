import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApplicationServieService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

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
    return this.http.post(`${this.baseUrl}Application/add-application/${programId}`, application);
  }
  GetApplicationForLoggedInUser(){
    return this.http.get(`${this.baseUrl}Application/get-applications-for-the-logged-in-user`);
  }
}
