import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserDataServiceService } from '../services/user-data-service.service';
import { catchError, finalize, map, of, take, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { DocumentService} from '../services/document.service';

// Remove duplicate DocumentDto interface since we're importing it from document.service

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
  //documents: DocumentDto[];
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
  isLoading = false;
  baseUrl = '';
  hasData = false;

  // Remove duplicate constructor and merge the properties
  constructor(
    private userDataService: UserDataServiceService,
    private http: HttpClient,
    private documentService: DocumentService
  ) {
    this.baseUrl = this.userDataService.baseUrl;
  }

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
  cvExists = false;
  baccalaureatExists = false;
  BaccalaureatGradesExists = false;
  BachelorExists = false;
  BachelorGradesExists = false;

  ngOnInit(): void {
    this.loadUserData();
    //CV
    this.documentService.CheckIfDocExist('CV').subscribe({
      next: result => {
        this.cvExists = result;
      },
      error: error => {
        console.error('Error checking document existence:', error);
        this.cvExists = false; // This return false equivalent makes sense here
      }
    });
    //Baccalaureat Degree
    this.documentService.CheckIfDocExist('Baccalaureat').subscribe({
      next: result => {
        this.baccalaureatExists = result;
      },
      error: error => {
        console.error('Error checking document existence:', error);
        this.baccalaureatExists = false; // This return false equivalent makes sense here
      }
    });

    //Baccalaureat Grades
    this.documentService.CheckIfDocExist('BaccalaureatGrades').subscribe({
      next: result => {
        this.BaccalaureatGradesExists = result;
      },
      error: error => {
        console.error('Error checking document existence:', error);
        this.BaccalaureatGradesExists = false; // This return false equivalent makes sense here
      }
    });
    //Bachelor
    this.documentService.CheckIfDocExist('Bachelor').subscribe({
      next: result => {
        this.BachelorExists = result;
      },
      error: error => {
        console.error('Error checking document existence:', error);
        this.BachelorExists = false; // This return false equivalent makes sense here
      }
    });

    //Bachelor Grades
    this.documentService.CheckIfDocExist('BachelorGrades').subscribe({
      next: result => {
        this.BachelorGradesExists = result;
      },
      error: error => {
        console.error('Error checking document existence:', error);
        this.BachelorGradesExists = false; // This return false equivalent makes sense here
      }
    });
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
    if (!documentType) return 'fas fa-file';

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
    if (!documentType) return '#6b7280';

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
  //checkIfDocExist(documentName: string): boolean {
  //  let exists = false;
  //  this.documentService.CheckIfDocExist(documentName).subscribe({
  //    next: result => {
  //      exists = result; // This happens AFTER the return statement
  //    }
  //  });
  //  return exists; // This ALWAYS returns false
  //}
  DeleteDocByName(documentName: string): void {
    this.documentService.DeleteDocByName(documentName).subscribe({
      next: result => {
        if (result) {
          console.log('Document deleted successfully:', documentName);
          this.loadUserData(); // Reload data to show updated information
          if (documentName === 'CV') {
            this.cvExists = false;
        }
        else if (documentName === 'Baccalaureat') {
          this.baccalaureatExists = false;
        }
        else if (documentName === 'BaccalaureatGrades') {
          this.BaccalaureatGradesExists = false;
        }
         else if (documentName === 'Bachelor') {
          this.BachelorExists = false;
        }
        else if (documentName === 'BachelorGrades') {
          this.BachelorGradesExists = false;
        }
        } else {
          console.error('Failed to delete document.');
        }
      },
      error: error => {
        console.error('Error deleting document:', error);
      }
    });
  }




  //*add
  selectedFile: File | null = null;
  documentName: string = '';
  isUploading: boolean = false;
  uploadSuccess: boolean = false;
  uploadMessage: string = '';

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
    // Reset messages when a new file is selected
    this.uploadSuccess = false;
    this.uploadMessage = '';
  }

  uploadDocument(name: string) {
    if (!this.selectedFile) {
      this.uploadMessage = 'Please select a file first';
      return;
    }

    // Set uploading state
    this.isUploading = true;
    this.uploadMessage = 'Uploading document...';

    this.documentService.AddDocument(this.selectedFile, name).subscribe({
      next: (response) => {
        console.log('Success:', response);
        // Reset form and show success message
        this.selectedFile = null;
        this.documentName = '';
        this.isUploading = false;
        this.uploadSuccess = true;
        this.uploadMessage = 'Document uploaded successfully!';
        
        // If this is a CV upload, update the CV exists flag
        if (name === 'CV') {
          this.cvExists = true;
        }
        else if (name === 'Baccalaureat') {
          this.baccalaureatExists = true;
        }
        else if (name === 'BaccalaureatGrades') {
          this.BaccalaureatGradesExists = true;
        }
         else if (name === 'Bachelor') {
          this.BachelorExists = true;
        }
        else if (name === 'BachelorGrades') {
          this.BachelorGradesExists = true;
        }
      },
      error: (error) => {
        // Handle error appropriately
        this.isUploading = false;
        this.uploadSuccess = false;
        this.handleUploadError(error);
      }
    });
  }

  private handleUploadError(error: any) {
    let errorMessage = 'Upload failed. Please try again.';
    
    if (error.status === 400) {
      // Handle specific error messages from the backend
      errorMessage = typeof error.error === 'string' ? error.error : 'Invalid file or missing information.';
    } else if (error.status === 401) {
      errorMessage = 'Please login to upload documents.';
      // Redirect to login page or show login modal if needed
    } else if (error.status === 500) {
      errorMessage = 'Server error. Please try again later.';
    }
    
    // Update the upload message to show the error
    this.uploadMessage = errorMessage;
    console.error(errorMessage);
  }

  
}

