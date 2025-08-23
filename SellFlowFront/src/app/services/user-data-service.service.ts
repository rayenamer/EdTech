import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BehaviorSubject, catchError, of, tap, finalize } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserDataServiceService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  // BehaviorSubject to store hasData value
  private hasDataSubject = new BehaviorSubject<boolean>(false);
  hasData$ = this.hasDataSubject.asObservable();
  
  // Caching mechanism to prevent multiple calls
  private hasDataLoaded = false;
  private loadingSubject = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();

  CheckUserHasData() {
    return this.http.get<{ hasData: boolean }>(`${this.baseUrl}UserData/check-user-has-data`);
  }

  checkUserDataOnce(): void {
    // Prevent multiple simultaneous calls
    if (this.hasDataLoaded || this.loadingSubject.value) {
      return;
    }

    this.loadingSubject.next(true);
    this.CheckUserHasData().pipe(
      tap(response => {
        console.log(response.hasData ? 'User has data.' : 'User does not have data.');
        this.hasDataLoaded = true; // Mark as loaded
      }),
      catchError(error => {
        console.error('Error checking user data:', error);
        this.loadingSubject.next(false);
        return of({ hasData: false });
      }),
      finalize(() => this.loadingSubject.next(false))
    ).subscribe(response => this.hasDataSubject.next(response.hasData));
  }

  // Reset cache when user logs out or data changes
  resetCache(): void {
    this.hasDataLoaded = false;
    this.hasDataSubject.next(false);
  }

  getMyUserData(){
    return this.http.get(`${this.baseUrl}UserData/get-user-UserDatas`).pipe(
      tap(response => console.log('User data retrieved successfully:', response)),
      catchError(error => {
        console.error('Error retrieving user data:', error);
        return of(null);
      })
    );
  }

  AddOrUpdatePersonalInformation(data: any) {
    return this.http.post(`${this.baseUrl}UserData/add/update-personal-information`, data).pipe(
      tap(response => console.log('Personal information updated successfully:', response)),
      catchError(error => {
        console.error('Error updating personal information:', error);
        return of(null);
      })
    );
  }
  AddOrUpdatePersonalStatements(data: any) {
    return this.http.post(`${this.baseUrl}UserData/add/update-personal-statements`, data).pipe(
      tap(response => console.log('Personal statements updated successfully:', response)),
      catchError(error => {
        console.error('Error updating personal statements:', error);
        return of(null);
      })
    );
  }
  AddOrUpdateEducationBackground(data: any) {
    return this.http.post(`${this.baseUrl}UserData/add/update-education-background`, data).pipe(
      tap(response => console.log('Education background updated successfully:', response)),
      catchError(error => {
        console.error('Error updating education background:', error);
        return of(null);
      })
    );
  }

  AddOrUpdateWorkExperience(data: any) {
    return this.http.post(`${this.baseUrl}UserData/add/update-work-experience`, data).pipe(
      tap(response => console.log('Work experience updated successfully:', response)),
      catchError(error => {
        console.error('Error updating work experience:', error);
        return of(null);
      })
    );
  }

 



  GetAllData() {
    return this.http.get(`${this.baseUrl}UserData/get-all-UserDatas`).pipe(
      tap(response => console.log('All user data retrieved successfully:', response)),
      catchError(error => {
        console.error('Error retrieving all user data:', error);
        return of(null);
      })
    );
  }

}
