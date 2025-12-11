import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UniProgram } from '../models/UniProgram';
import { UniProgramService } from '../services/uni-program.service';
import { ProgramCardComponent } from '../programs/program-card/program-card.component';
import { UserDataServiceService } from '../services/user-data-service.service';
import { catchError, map, Observable, of, shareReplay, take } from 'rxjs';

@Component({
  selector: 'app-acceuil',
  standalone: true,
  imports: [CommonModule, RouterModule, ProgramCardComponent],
  templateUrl: './acceuil.component.html',
  styleUrl: './acceuil.component.css'
})
export class AcceuilComponent implements OnInit {
  hasData: boolean = true; // store actual boolean, not Observable
  programs: UniProgram[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private uniProgramService: UniProgramService, 
    private userDataService: UserDataServiceService
  ) { }

  ngOnInit(): void {
    this.loadPrograms();
    this.userDataService.checkUserDataOnce(); // triggers the request
    this.userDataService.hasData$.subscribe(data => {
      this.hasData = data;
    });
  }

  completeProfile(): void {
    console.log('Complete profile clicked');
    // Navigation is handled by routerLink in template
  }

  viewProfile(): void {
    console.log('View profile clicked');
    // TODO: Navigate to profile view page
    // This could navigate to a profile details page
  }

  loadPrograms(): void {
    this.loading = true;
    this.error = null;

    this.uniProgramService.getPrograms().subscribe({
      next: (programs) => {
        this.programs = programs || [];
        this.loading = false;
        console.log('Programs loaded successfully:', programs);
      },
      error: (error) => {
        console.error('Error loading programs:', error);
        this.error = 'Failed to load programs. Please try again later.';
        this.loading = false;
      }
    });
  }

  retryLoad(): void {
    this.loadPrograms();
  }

  onProgramApply(program: UniProgram): void {
    console.log('Apply clicked for program:', program.name);
    // TODO: Implement application logic
    // This could open a modal, navigate to application form, etc.
  }

  onProgramLearnMore(program: UniProgram): void {
    console.log('Learn more clicked for program:', program.name);
    // TODO: Implement learn more logic
    // This could open a detailed view, navigate to program details page, etc.
  }

  getProgramById(id: number): void {
    this.uniProgramService.getProgramById(id).subscribe({
      next: (program) => {
        console.log('Program details loaded successfully:', program);
        // TODO: Implement logic to display program details
      },
      error: (error) => {
        console.error('Error loading program details:', error);
        // TODO: Implement error handling
      }
    });
  }
}
