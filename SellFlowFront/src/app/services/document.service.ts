import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, catchError, map, of, tap, throwError } from 'rxjs';

// Document interface matching the backend model
export interface DocumentDto {
  id: number;
  name?: string;
  fileName?: string;
  uploadDate?: string;
  documentType?: string;
  content?: string;
  size?: number;
  userDataId: number;
  downloadUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  uploadDocument(file: File, documentType: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    
    // Add document type as metadata in the file name
    const fileNameWithType = `${documentType}_${file.name}`;
    const renamedFile = new File([file], fileNameWithType, { type: file.type });
    formData.set('file', renamedFile);
    
    return this.http.post(`${this.baseUrl}Document/add-document`, formData).pipe(
      tap(response => console.log('Document uploaded successfully:', response)),
      catchError(error => {
        console.error('Error uploading document:', error);
        return of(null);
      })
    );
  }

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

  getAllDocuments(): Observable<DocumentDto[]> {
    return this.http.get<DocumentDto[]>(`${this.baseUrl}Document`).pipe(
      tap(response => console.log('Documents retrieved successfully:', response)),
      catchError(error => {
        console.error('Error retrieving documents:', error);
        return of([]);
      })
    );
  }
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

  return this.http.post<any>(`${this.baseUrl}Document/add-document`, formData).pipe(
    tap(response => console.log('Document uploaded successfully:', response)),
    catchError(error => {
      console.error('Error uploading document:', error);
      return throwError(() => error);
    })
  );
}
}