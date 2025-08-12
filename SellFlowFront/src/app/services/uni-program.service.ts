import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {UniProgram} from '../models/UniProgram';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UniProgramService {

  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  program: UniProgram | undefined;
  getPrograms() {
    return this.http.get<UniProgram[]>(`${this.baseUrl}UniProgram/get-programs`);
  }

  getProgramById(id: number) {
    return this.http.get<UniProgram>(`${this.baseUrl}UniProgram/get-program/${id}`);
  }
  addProgram(program: UniProgram) {
    return this.http.post<UniProgram>(`${this.baseUrl}UniProgram/add-program`, program);
  }
  deleteProgram(id: number) {
    return this.http.delete(`${this.baseUrl}UniProgram/delete-program/${id}`);
  }

}
