import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserDataServiceService } from '../services/user-data-service.service';
import { catchError, map, of, take } from 'rxjs';

interface DocumentDto {
  id: number;
  name: string;
  uploadDate: string;
  documentType: string;
  content: string;
  userDataId: number;
}

interface UserDataDto {
  id: number;
  fullName: string;
  number: string;
  dateOfBirth: string;
  motivation: string;
  lifeOutSide: string;
  baccalaureatDegree: string;
  baccalaureatInstitution: string;
  baccalaureatDate: string;
  bachelorDegree: string;
  bachelorInstitution: string;
  bachelorDate: string;
  masterDegree: string;
  masterInstitution: string;
  masterDate: string;
  engDegree: string;
  engInstitution: string;
  engDate: string;
  workExperience: string;
  linkedinLink: string;
  userId: number;
  documents: DocumentDto[];
}

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent implements OnInit {
  userData: UserDataDto | null = null;
  loading = false;
  errorMessage = '';
  hasData = false;
  
  // Personal Information Edit Mode
  isEditingPersonalInfo = false;
  personalInfoForm = {
    fullName: '',
    number: '',
    dateOfBirth: '',
    linkedinLink: ''
  };

  constructor(private userDataService: UserDataServiceService) { }

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    this.loading = true;
    this.errorMessage = '';

    this.userDataService.getMyUserData().pipe(
      take(1),
      map(response => {
        this.loading = false;
        
        if (response && Array.isArray(response) && response.length > 0) {
          // API returns an array, so we take the first item
          this.userData = response[0] as UserDataDto;
          this.hasData = true;
        } else if (response && !Array.isArray(response)) {
          // Fallback: if it's a single object
          this.userData = response as UserDataDto;
          this.hasData = true;
        } else {
          this.hasData = false;
        }
        return response;
      }),
      catchError(error => {
        this.loading = false;
        this.errorMessage = 'Failed to load user data. Please try again.';
        console.error('Error loading user data:', error);
        this.hasData = false;
        return of(null);
      })
    ).subscribe();
  }

  downloadDocument(documentId: number, documentName: string): void {
    // Create a temporary link element to trigger download
    const link = document.createElement('a');
    link.href = `${this.userDataService.baseUrl}UserData/download-document/${documentId}`;
    link.download = documentName;
    link.target = '_blank';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  getDocumentTypeIcon(documentType: string): string {
    switch (documentType.toLowerCase()) {
      case 'pdf':
        return 'fas fa-file-pdf';
      case 'image':
      case 'jpg':
      case 'jpeg':
      case 'png':
        return 'fas fa-file-image';
      case 'document':
      case 'doc':
      case 'docx':
        return 'fas fa-file-word';
      default:
        return 'fas fa-file';
    }
  }

  getDocumentTypeColor(documentType: string): string {
    switch (documentType.toLowerCase()) {
      case 'pdf':
        return '#dc2626';
      case 'image':
      case 'jpg':
      case 'jpeg':
      case 'png':
        return '#059669';
      case 'document':
      case 'doc':
      case 'docx':
        return '#2563eb';
      default:
        return '#6b7280';
    }
  }
  AddOrUpdatePersonalInformation(data: any): void {
    this.userDataService.AddOrUpdatePersonalInformation(data).subscribe({
      next: response => {
        if (response) {
          console.log('Personal information updated successfully:', response);
        } else {
          console.error('Failed to update personal information.');
        }
      },
      error: error => {
        console.error('Error updating personal information:', error);
      }
    });
  }

  // Personal Information Edit Methods
  startEditingPersonalInfo(): void {
    this.isEditingPersonalInfo = true;
    this.personalInfoForm = {
      fullName: this.userData?.fullName || '',
      number: this.userData?.number || '',
      dateOfBirth: this.userData?.dateOfBirth ? this.formatDateForInput(this.userData.dateOfBirth) : '',
      linkedinLink: this.userData?.linkedinLink || ''
    };
  }

  cancelEditingPersonalInfo(): void {
    this.isEditingPersonalInfo = false;
    this.personalInfoForm = {
      fullName: '',
      number: '',
      dateOfBirth: '',
      linkedinLink: ''
    };
  }

  savePersonalInfo(): void {
    if (!this.personalInfoForm.fullName.trim()) {
      this.errorMessage = 'Full name is required';
      return;
    }

    const personalInfoData = {
      fullName: this.personalInfoForm.fullName.trim(),
      number: this.personalInfoForm.number.trim(),
      dateOfBirth: this.personalInfoForm.dateOfBirth ? new Date(this.personalInfoForm.dateOfBirth + 'T00:00:00') : null,
      linkedinLink: this.personalInfoForm.linkedinLink.trim() || null
    };

    console.log('Sending personal info data:', personalInfoData);
    console.log('Date of birth type:', typeof personalInfoData.dateOfBirth);
    if (personalInfoData.dateOfBirth) {
      console.log('Date of birth value:', personalInfoData.dateOfBirth.toISOString());
    }

    this.userDataService.AddOrUpdatePersonalInformation(personalInfoData).subscribe({
      next: response => {
        if (response) {
          console.log('Personal information updated successfully:', response);
          this.isEditingPersonalInfo = false;
          this.loadUserData(); // Reload data to show updated information
          this.errorMessage = '';
        } else {
          console.error('Failed to update personal information.');
          this.errorMessage = 'Failed to update personal information. Please try again.';
        }
      },
      error: error => {
        console.error('Error updating personal information:', error);
        if (error.error && typeof error.error === 'string') {
          this.errorMessage = error.error;
        } else {
          this.errorMessage = 'Error updating personal information. Please try again.';
        }
      }
    });
  }

  formatDateForInput(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toISOString().split('T')[0]; // Format as YYYY-MM-DD for input
  }
}
