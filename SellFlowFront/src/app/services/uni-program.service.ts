import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {UniProgram} from '../models/UniProgram';
import { environment } from '../../environments/environment';
import { of } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UniProgramService {

  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  program: UniProgram | undefined;
  
  private readonly CACHE_KEY_ALL = 'university_programs';
  private readonly CACHE_KEY_SINGLE = 'program_';

  getPrograms() {
    // Check sessionStorage first (only valid for current session)
    const cached = sessionStorage.getItem(this.CACHE_KEY_ALL);
    if (cached) {
      return of(JSON.parse(cached) as UniProgram[]);
    }

    // If not cached, fetch from API
    return this.http.get<UniProgram[]>(`${this.baseUrl}UniProgram/get-programs`).pipe(
      tap(programs => {
        // Store in sessionStorage (cleared when browser closes)
        sessionStorage.setItem(this.CACHE_KEY_ALL, JSON.stringify(programs));
      })
    );
  }

  getProgramById(id: number) {
    const cacheKey = `${this.CACHE_KEY_SINGLE}${id}`;
    
    // Check sessionStorage first
    const cached = sessionStorage.getItem(cacheKey);
    if (cached) {
      return of(JSON.parse(cached) as UniProgram);
    }

    // If not cached, fetch from API
    return this.http.get<UniProgram>(`${this.baseUrl}UniProgram/get-program/${id}`).pipe(
      tap(program => {
        // Store in sessionStorage
        sessionStorage.setItem(cacheKey, JSON.stringify(program));
      })
    );
  }

  addProgram(program: UniProgram) {
    return this.http.post<UniProgram>(`${this.baseUrl}UniProgram/add-program`, program);
  }

  deleteProgram(id: number) {
    return this.http.delete(`${this.baseUrl}UniProgram/delete-program/${id}`);
  }

  // Clear cache on logout
  clearCache() {
    sessionStorage.removeItem(this.CACHE_KEY_ALL);
    const keys = Object.keys(sessionStorage);
    keys.forEach(key => {
      if (key.startsWith(this.CACHE_KEY_SINGLE)) {
        sessionStorage.removeItem(key);
      }
    });
  }

}
