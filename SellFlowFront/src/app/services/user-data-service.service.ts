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
    // Convert JSON data to FormData since the backend expects [FromForm]
    const formData = new FormData();
    
    // Add all the user data fields
    Object.keys(data).forEach(key => {
      if (key === 'documents') {
        // Handle documents array specially
        if (data[key] && Array.isArray(data[key])) {
          data[key].forEach((doc: any, index: number) => {
            formData.append(`documents[${index}].id`, doc.id);
            formData.append(`documents[${index}].name`, doc.name);
            formData.append(`documents[${index}].documentType`, doc.documentType);
            formData.append(`documents[${index}].uploadDate`, doc.uploadDate);
            formData.append(`documents[${index}].userDataId`, doc.userDataId);
          });
        }
      } else if (data[key] !== null && data[key] !== undefined) {
        formData.append(key, data[key].toString());
      }
    });

    console.log('Sending FormData:', formData);
    return this.http.post(`${this.baseUrl}UserData/add-UserData`, formData);
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

  getMyUserData(){
    return this.http.get(`${this.baseUrl}UserData/get-user-UserDatas`).pipe(
      tap(response => console.log('User data retrieved successfully:', response)),
      catchError(error => {
        console.error('Error retrieving user data:', error);
        return of(null);
      })
    );
  }

}
