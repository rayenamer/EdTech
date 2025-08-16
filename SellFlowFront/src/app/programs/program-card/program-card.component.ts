import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UniProgram } from '../../models/UniProgram';
import { routes } from '../../app.routes';
import { UserDataServiceService } from '../../services/user-data-service.service';

@Component({
  selector: 'app-program-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './program-card.component.html',
  styleUrl: './program-card.component.css'
})
export class ProgramCardComponent implements OnInit {
  hasData: boolean = true; // store actual boolean, not Observable
  @Input() program!: UniProgram;
  @Output() applyClicked = new EventEmitter<UniProgram>();
  @Output() learnMoreClicked = new EventEmitter<UniProgram>();

  private universityImage: string = '';
  
  constructor(private router: Router,private userDataService: UserDataServiceService) {}
  
  ngOnInit(): void {
    this.universityImage = this.getRandomUniversityImage();
    this.userDataService.checkUserDataOnce(); // triggers the request
    this.userDataService.hasData$.subscribe(data => {
      this.hasData = data;
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
      year: 'numeric' 
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

  onLearnMoreClick(): void {
    // Navigate to program details with the program ID
    this.router.navigate(['/programdetails', this.program.id]);
  }
  
}
