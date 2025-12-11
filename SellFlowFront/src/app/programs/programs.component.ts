import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProgramCardComponent } from './program-card';
import { UniProgram } from '../models/UniProgram';
import { UniProgramService } from '../services/uni-program.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-programs',
  standalone: true,
  imports: [CommonModule, ProgramCardComponent],
  templateUrl: './programs.component.html',
  styleUrl: './programs.component.css'
})
export class ProgramsComponent implements OnInit {
  programs: UniProgram[] = [];
  loading = false;
  error: string | null = null;
  deletingProgramId: number | null = null;
  constructor(
    private uniProgramService: UniProgramService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPrograms();
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


  deleteProgram(programId: number): void {
    this.deletingProgramId = programId;
    this.uniProgramService.deleteProgram(programId).subscribe({
      next: () => {
        this.programs = this.programs.filter(program => program.id !== programId);
        this.deletingProgramId = null;
      },
      error: (error) => {
        console.error('Error deleting program:', error);
        this.error = 'Failed to delete program. Please try again later.';
        this.deletingProgramId = null;
      }
    });
  }

  confirmDelete(program: UniProgram): void {
    if (confirm(`Are you sure you want to delete "${program.name}"? This action cannot be undone.`)) {
      this.deleteProgram(program.id);
    }
  }
  GoToAddProgram(): void {
    // Navigate to the add program page
    this.router.navigate(['/add-program']);
  }

}
