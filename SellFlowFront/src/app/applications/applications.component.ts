import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApplicationServieService } from '../services/application-servie.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './applications.component.html',
  styleUrl: './applications.component.css'
})
export class ApplicationsComponent implements OnInit {
  applications: any[] = [];
  applicationStates: string[] = ['Submitted', 'Under Review', 'Accepted', 'Rejected'];
  selectedState: string = '';
  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private applicationService: ApplicationServieService) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.loading = true;
    this.applicationService.getApplications().subscribe({
      next: (response: any) => {
        this.applications = response;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading applications:', error);
        this.errorMessage = 'Failed to load applications. Please try again.';
        this.loading = false;
      }
    });
  }

  changeState(applicationId: number, state: string): void {
    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';
    
    this.applicationService.changeApplicationState(applicationId, state).subscribe({
      next: () => {
        this.successMessage = `Application status changed to ${state}`;
        this.loadApplications(); // Reload the applications to get updated data
      },
      error: (error) => {
        console.error('Error changing application state:', error);
        this.errorMessage = 'Failed to change application state. Please try again.';
        this.loading = false;
      }
    });
  }
}
