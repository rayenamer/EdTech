import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, catchError, map, of, tap, throwError } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  deleteDocument(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}Document/${id}`).pipe(
      tap(response => console.log('Document deleted successfully:', response)),
      catchError(error => {
        console.error('Error deleting document:', error);
        return of(null);
      })
    );
  }

  //for admin
  downloadDocument(id: number, fileName: string = 'document'): string {
    return `${this.baseUrl}Document/download/${id}?fileName=${encodeURIComponent(fileName)}`;
  }
  //for admin
  CheckIfDocExist(documentName: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}Document/check-document-name-AND-user-id/${documentName}`).pipe(
      tap(response => console.log('Document existence checked successfully:', response)),
      catchError(error => {
        console.error('Error checking document existence:', error);
        return of(false);
      })
    );
  }
  DeleteDocByName(documentName: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.baseUrl}Document/DeleteDocByName/${documentName}`).pipe(
      tap(response => console.log('Document deleted successfully:', response)),
      catchError(error => {
        console.error('Error deleting document:', error);
        return of(false);
      })
    );
  }
  AddDocument(file: File, documentName: string): Observable<any> {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('documentName', documentName);

  return this.http.post<any>(`${this.baseUrl}Document/add-document/${documentName}`, formData).pipe(
    tap(response => console.log('Document uploaded successfully:', response)),
    catchError(error => {
      console.error('Error uploading document:', error);
      return throwError(() => error);
    })
  );
}
}