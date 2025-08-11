import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UniProgram } from '../models/UniProgram';
import { UniProgramService } from '../services/uni-program.service';
import { ProgramCardComponent } from '../programs/program-card/program-card.component';

@Component({
  selector: 'app-acceuil',
  standalone: true,
  imports: [CommonModule, ProgramCardComponent],
  templateUrl: './acceuil.component.html',
  styleUrl: './acceuil.component.css'
})
export class AcceuilComponent implements OnInit {
  constructor(private uniProgramService: UniProgramService) {}
  
  ngOnInit(): void {
    this.loadPrograms();
  }

  programs: UniProgram[] = [];
  loading = false;
  error: string | null = null;
  
  loadPrograms(): void {
    this.loading = true;
    this.error = null;

    this.uniProgramService.getPrograms().subscribe({
      next: (programs) => {
        this.programs = programs;
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
}
