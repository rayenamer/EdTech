import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { UniProgram } from '../models/UniProgram';
import { UniProgramService } from '../services/uni-program.service';

@Component({
  selector: 'app-programdetailscard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './programdetailscard.component.html',
  styleUrl: './programdetailscard.component.css'
})
export class ProgramdetailscardComponent implements OnInit {
  @Input() program!: UniProgram;
  @Output() applyClicked = new EventEmitter<UniProgram>();
  @Output() backToListClicked = new EventEmitter<void>();

  
  constructor(
    private uniProgramService: UniProgramService,
    private route: ActivatedRoute,
    private router: Router
  ) {}
  private universityImage: string = '';

  ngOnInit(): void {
    this.universityImage = this.getRandomUniversityImage();
    
    // Get the program ID from route parameters
    this.route.params.subscribe(params => {
      const programId = params['id'];
      if (programId) {
        this.loadProgramDetails(programId);
      }
    });
  }

  getDurationText(): string {
    if (this.program.duration === 1) {
      return '1 month';
    }
    return `${this.program.duration} months`;
  }

  getFormattedStartDate(): string {
    const date = new Date(this.program.programStart);
    return date.toLocaleDateString('en-US', { 
      month: 'long', 
      year: 'numeric',
      day: 'numeric'
    });
  }

  getUniversityImage(): string {
    return this.universityImage;
  }

  private getRandomUniversityImage(): string {
    // Return a random university image
    const universityImages = [
      'https://images.unsplash.com/photo-1464207687429-7505649dae38?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1562774053-701939374585?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1499856871958-5b9627545d1a?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1574717024653-61fd2cf4d44d?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1523240798132-8751934a7ff8?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1554224155-6726b3ff858f?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1554224154-26032cdc0d0a?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1552664730-d307ca884978?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80'
    ];
    
    const randomIndex = Math.floor(Math.random() * universityImages.length);
    return universityImages[randomIndex];
  }

  onApplyClick(): void {
    this.applyClicked.emit(this.program);
  }

  onBackToListClick(): void {
    this.backToListClicked.emit();
  }

  private loadProgramDetails(programId: string): void {
    const id = parseInt(programId, 10);
    if (isNaN(id)) {
      console.error('Invalid program ID:', programId);
      return;
    }

    this.uniProgramService.getProgramById(id).subscribe({
      next: (program) => {
        console.log('Program details loaded successfully:', program);
        this.program = program;
      },
      error: (error) => {
        console.error('Error loading program details:', error);
        // TODO: Implement error handling
      }
    });
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
  onBrowseOtherProgramsClick(): void {
    this.router.navigate(['/Acceuil']);
  }
}
