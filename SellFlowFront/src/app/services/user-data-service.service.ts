import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BehaviorSubject, catchError, of, tap, finalize, Observable } from 'rxjs';

export interface DocumentStatusDto {
  cv: boolean;
  baccalaureat: boolean;
  baccalaureatGrades: boolean;
  bachelor: boolean;
  bachelorGrades: boolean;
}

export interface UserDataDto {
  id: number;
  fullName: string;
  number: string;
  dateOfBirth: string;
  motivation: string;
  lifeOutSide: string;
  baccalaureatDegree: string;
  baccalaureatInstitution: string;
  baccalaureatDate: string;
  bachelorDegree: string;
  bachelorInstitution: string;
  bachelorDate: string;
  masterDegree: string;
  masterInstitution: string;
  masterDate: string;
  engDegree: string;
  engInstitution: string;
  engDate: string;
  workExperience: string;
  linkedinLink: string;
  userId: number;
  documents: DocumentDto[];
}

export interface DocumentDto {
  id: number;
  userDataId: number;
  documentName: string;
  downloadUrl: string;
}

export interface UserDataWithDocumentsDto {
  userData: UserDataDto;
  documentStatus: DocumentStatusDto;
}

@Injectable({
  providedIn: 'root'
})
export class UserDataServiceService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  private readonly USER_DATA_CACHE_KEY = 'user_data_cache';
  private readonly HAS_DATA_CACHE_KEY = 'has_data_cache';

  // BehaviorSubject to store hasData value
  private hasDataSubject = new BehaviorSubject<boolean>(false);
  hasData$ = this.hasDataSubject.asObservable();
  
  // Loading state
  private loadingSubject = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();

  CheckUserHasData() {
    // Check cache first
    const cached = sessionStorage.getItem(this.HAS_DATA_CACHE_KEY);
    if (cached) {
      return of(JSON.parse(cached));
    }

    // Fetch from server if not in cache
    return this.http.get<{ hasData: boolean }>(`${this.baseUrl}UserData/check-user-has-data`).pipe(
      tap(response => {
        sessionStorage.setItem(this.HAS_DATA_CACHE_KEY, JSON.stringify(response));
      })
    );
  }

  checkUserDataOnce(): void {
    // Prevent multiple simultaneous calls
    if (this.loadingSubject.value) {
      return;
    }

    this.loadingSubject.next(true);
    this.CheckUserHasData().pipe(
      tap(response => {
        console.log(response.hasData ? 'User has data.' : 'User does not have data.');
        this.hasDataSubject.next(response.hasData);
      }),
      catchError(error => {
        console.error('Error checking user data:', error);
        this.loadingSubject.next(false);
        return of({ hasData: false });
      }),
      finalize(() => this.loadingSubject.next(false))
    ).subscribe();
  }

  // Clear all caches
  clearCache(): void {
    sessionStorage.removeItem(this.USER_DATA_CACHE_KEY);
    sessionStorage.removeItem(this.HAS_DATA_CACHE_KEY);
    this.hasDataSubject.next(false);
  }

  // Reset cache when user logs out or data changes
  resetCache(): void {
    this.clearCache();
  }

  // Invalidate cache when user data changes
  private invalidateUserDataCache(): void {
    sessionStorage.removeItem(this.USER_DATA_CACHE_KEY);
    sessionStorage.removeItem(this.HAS_DATA_CACHE_KEY);
    this.hasDataSubject.next(false);
  }

  getMyUserData() {
  // Check cache first
  const cached = sessionStorage.getItem(this.USER_DATA_CACHE_KEY);
  if (cached) {
    try {
      const parsedData = JSON.parse(cached);
      console.log('Using cached user data');
      return of(parsedData);
    } catch (e) {
      console.error('Error parsing cached user data:', e);
      sessionStorage.removeItem(this.USER_DATA_CACHE_KEY);
    }
  }

  // Using the correct endpoint from your logs
  return this.http.get(`${this.baseUrl}UserData/get-user-data-with-documents`).pipe(
    tap(response => {
      if (response) {
        console.log('Caching user data with documents');
        sessionStorage.setItem(this.USER_DATA_CACHE_KEY, JSON.stringify(response));
      }
    }),
    catchError(error => {
      console.error('Error retrieving user data:', error);
      return of(null);
    })
  );
}
  

  AddOrUpdatePersonalInformation(data: any) {
    this.clearCache();
    return this.http.post(`${this.baseUrl}UserData/add/update-personal-information`, data).pipe(
      tap(response => console.log('Personal information updated successfully:', response)),
      catchError(error => {
        console.error('Error updating personal information:', error);
        return of(null);
      })
    );
  }
  AddOrUpdatePersonalStatements(data: any) {
    this.clearCache();
    return this.http.post(`${this.baseUrl}UserData/add/update-personal-statements`, data).pipe(
      tap(response => console.log('Personal statements updated successfully:', response)),
      catchError(error => {
        console.error('Error updating personal statements:', error);
        return of(null);
      })
    );
  }

  AddOrUpdateEducationBackground(data: any) {

    this.clearCache();
    return this.http.post(`${this.baseUrl}UserData/add/update-education-background`, data).pipe(
      tap(response => console.log('Education background updated successfully:', response)),
      catchError(error => {
        console.error('Error updating education background:', error);
        return of(null);
      })
    );
  }

  AddOrUpdateWorkExperience(data: any) {

    this.clearCache();
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

  // NEW OPTIMIZED METHOD - Single API call
  // NEW OPTIMIZED METHOD - Single API call
getMyUserDataWithDocuments(): Observable<UserDataWithDocumentsDto> {
  // Check cache first
  const cached = sessionStorage.getItem(this.USER_DATA_CACHE_KEY);
  if (cached) {
    try {
      const parsedData = JSON.parse(cached);
      console.log('Using cached user data with documents');
      return of(parsedData);
    } catch (e) {
      console.error('Error parsing cached user data:', e);
      sessionStorage.removeItem(this.USER_DATA_CACHE_KEY);
    }
  }

  // If not in cache, fetch from server
  return this.http.get<UserDataWithDocumentsDto>(`${this.baseUrl}UserData/get-user-data-with-documents`).pipe(
    tap(response => {
      if (response) {
        console.log('Caching user data with documents');
        sessionStorage.setItem(this.USER_DATA_CACHE_KEY, JSON.stringify(response));
      }
    }),
    catchError(error => {
      console.error('Error retrieving user data with documents:', error);
      return of({
        userData: null as any,
        documentStatus: {
          cv: false,
          baccalaureat: false,
          baccalaureatGrades: false,
          bachelor: false,
          bachelorGrades: false
        }
      } as UserDataWithDocumentsDto);
    })
  );
}

}
