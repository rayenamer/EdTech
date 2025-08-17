import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserDataServiceService } from '../services/user-data-service.service';
import { catchError, finalize, map, of, take } from 'rxjs';
import { HttpClient } from '@angular/common/http';

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
uploadDocument() {
    // Create a file input element
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = '.pdf,.jpg,.jpeg,.png';
    
    // Trigger file dialog
    fileInput.click();
    
    // Handle file selection
    fileInput.addEventListener('change', (event) => {
      const target = event.target as HTMLInputElement;
      if (target.files && target.files.length > 0) {
        const file = target.files[0];
        
        // Prompt user to select document type
        const documentType = prompt('Select document type:', 'CV, Bachelor Degree, Bachelor Grades, Other');
        if (!documentType) return; // User cancelled
        
        // Create form data
        const formData = new FormData();
        formData.append('files', file);
        formData.append('documentType', documentType);
        
        // Show loading indicator
        this.isLoading = true;
        
        // Send to API
        this.http.post(`${this.baseUrl}UserData/add-UserData`, formData)
          .pipe(
            finalize(() => this.isLoading = false)
          )
          .subscribe({
            next: () => {
              alert('Document uploaded successfully!');
              this.loadUserData(); // Refresh data
            },
            error: (error) => {
              console.error('Error uploading document:', error);
              alert('Failed to upload document. ' + (error.error || 'Please try again.'));
            }
          });
      }
    });
  }
  
  
  userData: UserDataDto | null = null;
  loading = false;
  errorMessage = '';
  isLoading = false;
  baseUrl = '';
  
  constructor(private userDataService: UserDataServiceService, private http: HttpClient) {
    this.baseUrl = this.userDataService.baseUrl;
  }
  hasData = false;
  
  // Personal Information Edit Mode
  isEditingPersonalInfo = false;
  personalInfoForm = {
    fullName: '',
    number: '',
    dateOfBirth: '',
    linkedinLink: ''
  };

  // Personal Statements Edit Mode
  isEditingPersonalStatements = false;
  personalStatementsForm = {
    motivation: '',
    lifeOutSide: ''
  };

  // Education Background Edit Mode
  isEditingEducation = false;
  educationForm = {
    baccalaureatDegree: '',
    baccalaureatInstitution: '',
    baccalaureatDate: '',
    bachelorDegree: '',
    bachelorInstitution: '',
    bachelorDate: '',
    masterDegree: '',
    masterInstitution: '',
    masterDate: '',
    engDegree: '',
    engInstitution: '',
    engDate: ''
  };

  // Work Experience Edit Mode
  isEditingWorkExperience = false;
  workExperienceForm = {
    workExperience: ''
  };



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

  // Document category filter methods
  getBaccalaureatDiploma(): DocumentDto[] {
    return this.userData?.documents?.filter(doc => 
      doc.documentType.toLowerCase().includes('baccalaureat') && 
      doc.documentType.toLowerCase().includes('diploma')
    ) || [];
  }

  getBaccalaureatGrades(): DocumentDto[] {
    return this.userData?.documents?.filter(doc => 
      doc.documentType.toLowerCase().includes('baccalaureat') && 
      doc.documentType.toLowerCase().includes('grades')
    ) || [];
  }

  getCV(): DocumentDto[] {
    return this.userData?.documents?.filter(doc => 
      doc.documentType.toLowerCase().includes('cv') || 
      doc.documentType.toLowerCase().includes('resume')
    ) || [];
  }

  getBachelorDegree(): DocumentDto[] {
    return this.userData?.documents?.filter(doc => 
      doc.documentType.toLowerCase().includes('bachelor') && 
      doc.documentType.toLowerCase().includes('degree')
    ) || [];
  }

  getBachelorGrades(): DocumentDto[] {
    return this.userData?.documents?.filter(doc => 
      doc.documentType.toLowerCase().includes('bachelor') && 
      doc.documentType.toLowerCase().includes('grades')
    ) || [];
  }

  getOtherFiles(): DocumentDto[] {
    return this.userData?.documents?.filter(doc => 
      !doc.documentType.toLowerCase().includes('baccalaureat') && 
      !doc.documentType.toLowerCase().includes('bachelor') && 
      !doc.documentType.toLowerCase().includes('cv') && 
      !doc.documentType.toLowerCase().includes('resume')
    ) || [];
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

  // Personal Statements Edit Methods
  startEditingPersonalStatements(): void {
    this.isEditingPersonalStatements = true;
    this.personalStatementsForm = {
      motivation: this.userData?.motivation || '',
      lifeOutSide: this.userData?.lifeOutSide || ''
    };
  }

  cancelEditingPersonalStatements(): void {
    this.isEditingPersonalStatements = false;
    this.personalStatementsForm = {
      motivation: '',
      lifeOutSide: ''
    };
  }

  savePersonalStatements(): void {
    if (!this.personalStatementsForm.motivation.trim()) {
      this.errorMessage = 'Motivation statement is required';
      return;
    }

    const personalStatementsData = {
      motivation: this.personalStatementsForm.motivation.trim(),
      lifeOutSide: this.personalStatementsForm.lifeOutSide.trim() || null
    };

    console.log('Sending personal statements data:', personalStatementsData);

    this.userDataService.AddOrUpdatePersonalStatements(personalStatementsData).subscribe({
      next: response => {
        if (response) {
          console.log('Personal statements updated successfully:', response);
          this.isEditingPersonalStatements = false;
          this.loadUserData(); // Reload data to show updated information
          this.errorMessage = '';
        } else {
          console.error('Failed to update personal statements.');
          this.errorMessage = 'Failed to update personal statements. Please try again.';
        }
      },
      error: error => {
        console.error('Error updating personal statements:', error);
        if (error.error && typeof error.error === 'string') {
          this.errorMessage = error.error;
        } else {
          this.errorMessage = 'Error updating personal statements. Please try again.';
        }
      }
    });
  }

  // Education Background Edit Methods
  startEditingEducation(): void {
    this.isEditingEducation = true;
    this.educationForm = {
      baccalaureatDegree: this.userData?.baccalaureatDegree || '',
      baccalaureatInstitution: this.userData?.baccalaureatInstitution || '',
      baccalaureatDate: this.userData?.baccalaureatDate ? this.formatDateForInput(this.userData.baccalaureatDate) : '',
      bachelorDegree: this.userData?.bachelorDegree || '',
      bachelorInstitution: this.userData?.bachelorInstitution || '',
      bachelorDate: this.userData?.bachelorDate ? this.formatDateForInput(this.userData.bachelorDate) : '',
      masterDegree: this.userData?.masterDegree || '',
      masterInstitution: this.userData?.masterInstitution || '',
      masterDate: this.userData?.masterDate ? this.formatDateForInput(this.userData.masterDate) : '',
      engDegree: this.userData?.engDegree || '',
      engInstitution: this.userData?.engInstitution || '',
      engDate: this.userData?.engDate ? this.formatDateForInput(this.userData.engDate) : ''
    };
  }

  cancelEditingEducation(): void {
    this.isEditingEducation = false;
    this.educationForm = {
      baccalaureatDegree: '',
      baccalaureatInstitution: '',
      baccalaureatDate: '',
      bachelorDegree: '',
      bachelorInstitution: '',
      bachelorDate: '',
      masterDegree: '',
      masterInstitution: '',
      masterDate: '',
      engDegree: '',
      engInstitution: '',
      engDate: ''
    };
  }

  saveEducation(): void {
    const educationData = {
      baccalaureatDegree: this.educationForm.baccalaureatDegree.trim() || null,
      baccalaureatInstitution: this.educationForm.baccalaureatInstitution.trim() || null,
      baccalaureatDate: this.educationForm.baccalaureatDate ? new Date(this.educationForm.baccalaureatDate + 'T00:00:00') : null,
      bachelorDegree: this.educationForm.bachelorDegree.trim() || null,
      bachelorInstitution: this.educationForm.bachelorInstitution.trim() || null,
      bachelorDate: this.educationForm.bachelorDate ? new Date(this.educationForm.bachelorDate + 'T00:00:00') : null,
      masterDegree: this.educationForm.masterDegree.trim() || null,
      masterInstitution: this.educationForm.masterInstitution.trim() || null,
      masterDate: this.educationForm.masterDate ? new Date(this.educationForm.masterDate + 'T00:00:00') : null,
      engDegree: this.educationForm.engDegree.trim() || null,
      engInstitution: this.educationForm.engInstitution.trim() || null,
      engDate: this.educationForm.engDate ? new Date(this.educationForm.engDate + 'T00:00:00') : null
    };

    console.log('Sending education data:', educationData);

    this.userDataService.AddOrUpdateEducationBackground(educationData).subscribe({
      next: response => {
        if (response) {
          console.log('Education background updated successfully:', response);
          this.isEditingEducation = false;
          this.loadUserData(); // Reload data to show updated information
          this.errorMessage = '';
        } else {
          console.error('Failed to update education background.');
          this.errorMessage = 'Failed to update education background. Please try again.';
        }
      },
      error: error => {
        console.error('Error updating education background:', error);
        if (error.error && typeof error.error === 'string') {
          this.errorMessage = error.error;
        } else {
          this.errorMessage = 'Error updating education background. Please try again.';
        }
      }
    });
  }

  // Work Experience Edit Methods
  startEditingWorkExperience(): void {
    this.isEditingWorkExperience = true;
    this.workExperienceForm = {
      workExperience: this.userData?.workExperience || ''
    };
  }

  cancelEditingWorkExperience(): void {
    this.isEditingWorkExperience = false;
    this.workExperienceForm = {
      workExperience: ''
    };
  }

  saveWorkExperience(): void {
    const workExperienceData = {
      WorkExperience: this.workExperienceForm.workExperience.trim() || null
    };

    console.log('Sending work experience data:', workExperienceData);

    this.userDataService.AddOrUpdateWorkExperience(workExperienceData).subscribe({
      next: response => {
        if (response) {
          console.log('Work experience updated successfully:', response);
          this.isEditingWorkExperience = false;
          this.loadUserData(); // Reload data to show updated information
          this.errorMessage = '';
        } else {
          console.error('Failed to update work experience.');
          this.errorMessage = 'Failed to update work experience. Please try again.';
        }
      },
      error: error => {
        console.error('Error updating work experience:', error);
        if (error.error && typeof error.error === 'string') {
          this.errorMessage = error.error;
        } else {
          this.errorMessage = 'Error updating work experience. Please try again.';
        }
      }
    });
  }
}
