import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationServieService } from '../services/application-servie.service';

@Component({
  selector: 'app-add-application',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-application.component.html',
  styleUrl: './add-application.component.css'
})
export class AddApplicationComponent implements OnInit {
  programId: number = 0;
  public DoubleApplication: boolean = false;
  application = {
    WhyDidYouApply: ''
  };
  isSubmitting = false;
  errorMessage = '';

  constructor(
    
    private route: ActivatedRoute,
    private router: Router,
    private applicationService: ApplicationServieService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.programId = +params['id']; // Convert to number
    });
  }

  onSubmit(): void {
    if (!this.application.WhyDidYouApply.trim()) {
      this.errorMessage = 'Please explain why you are applying';
      return;
    }

    this.isSubmitting = true;
    this.applicationService.AddApplication(this.application, this.programId)
      .subscribe({
        next: () => {
          this.router.navigate(['/my-applications']);
        },
        error: (error) => {
        this.isSubmitting = false;
        
        // Extract the actual error message from the backend response
        if (error.error && typeof error.error === 'string') {
          //
          this.DoubleApplication = true;
          //
          this.errorMessage = error.error;
        } else if (error.error && error.error.message) {
          // Handle object error messages with message property
          this.errorMessage = error.error.message;
        } else if (error.error && error.error.title) {
          // Handle validation error responses
          this.errorMessage = error.error.title;
        } else if (error.message) {
          // Handle HTTP error messages
          this.errorMessage = error.message;
        } else {
          // Fallback to generic message
          this.errorMessage = 'Failed to submit application. Please try again.';
        }
        
        console.error('Application submission error:', error);
      }
      });
  }
}
