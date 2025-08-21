// my-applications.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApplicationServieService } from '../services/application-servie.service';

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-applications.component.html',
  styleUrl: './my-applications.component.css'
})
export class MyApplicationsComponent implements OnInit {
  applications: any[] = [];
  loading = true;
  error = '';

  constructor(private applicationService: ApplicationServieService) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.applicationService.GetApplicationForLoggedInUser()
      .subscribe({
        next: (data: any) => {
          // Handle both direct array and nested object structure
          this.applications = Array.isArray(data) ? data : (data.applications || []);
          this.loading = false;
          console.log('Applications loaded:', this.applications);
        },
        error: (error) => {
          this.error = 'Failed to load your applications. Please try again later.';
          this.loading = false;
          console.error('Error loading applications:', error);
        }
      });
  }

  getStatusClass(status: string | null | undefined): string {
    if (!status) return 'status-pending';
    
    switch (status.toLowerCase()) {
      case 'approved':
        return 'status-approved';
      case 'rejected':
        return 'status-rejected';
      case 'pending':
        return 'status-pending';
      default:
        return 'status-pending';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch {
      return dateString;
    }
  }
}