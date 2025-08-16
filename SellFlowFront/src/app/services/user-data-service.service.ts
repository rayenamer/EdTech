import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BehaviorSubject, catchError, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserDataServiceService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  // BehaviorSubject to store hasData value
  private hasDataSubject = new BehaviorSubject<boolean>(false);
  hasData$ = this.hasDataSubject.asObservable();

  CheckUserHasData() {
    return this.http.get<{ hasData: boolean }>(`${this.baseUrl}UserData/check-user-has-data`);
  }
  AddUserData(data: any) {
    return this.http.post(`${this.baseUrl}UserData/add-user-data`, data);
  }

  checkUserDataOnce(): void {
    this.CheckUserHasData().pipe(
      tap(response => console.log(response.hasData ? 'User has data.' : 'User does not have data.')),
      catchError(error => {
        console.error('Error checking user data:', error);
        return of({ hasData: false });
      })
    ).subscribe(response => this.hasDataSubject.next(response.hasData));
  }
}
