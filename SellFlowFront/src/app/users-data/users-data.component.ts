import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserDataServiceService } from '../services/user-data-service.service';

// Interface pour les données utilisateur
interface UserData {
  id: number;
  fullName: string;
  number: string;
  dateOfBirth: string;
  motivation: string;
  lifeOutSide: string;
  baccalaureatDegree: string;
  baccalaureatInstitution: string;
  baccalaureatDate: string;
  bachelorDegree: string | null;
  bachelorInstitution: string | null;
  bachelorDate: string | null;
  masterDegree: string | null;
  masterInstitution: string | null;
  masterDate: string | null;
  engDegree: string | null;
  engInstitution: string | null;
  engDate: string | null;
  workExperience: string | null;
  linkedinLink: string;
  userId: number;
  documents: Document[];
}

// Interface pour les documents
interface Document {
  id: number;
  bytes: any;
  userDataId: number;
  downloadUrl: string;
  documentName: string;
}

@Component({
  selector: 'app-users-data',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './users-data.component.html',
  styleUrls: ['./users-data.component.css']
})
export class UsersDataComponent implements OnInit {
  // Données des utilisateurs
  usersData: UserData[] = [];
  
  // États de l'interface
  isLoading: boolean = true;
  error: string | null = null;
  expandedUsers: number[] = [];

  constructor(private userDataService: UserDataServiceService) { }

  ngOnInit(): void {
    this.loadUsersData();
  }

  // Charger les données des utilisateurs
 loadUsersData(): void {
  this.isLoading = true;
  this.error = null;

  this.userDataService.GetAllData().subscribe({
    next: (data: any) => {
      this.usersData = data as UserData[];
      this.isLoading = false;
    },
    error: (err) => {
      console.error('Erreur lors du chargement des données utilisateurs', err);
      this.error = 'Impossible de charger les données utilisateurs. Veuillez réessayer plus tard.';
      this.isLoading = false;
    }
  });
}

  // Afficher/masquer les détails d'un utilisateur
  toggleUserDetails(userId: number): void {
    if (this.expandedUsers.includes(userId)) {
      this.expandedUsers = this.expandedUsers.filter(id => id !== userId);
    } else {
      this.expandedUsers.push(userId);
    }
  }

  // Formater les dates pour l'affichage
  formatDate(dateString: string | null): string {
    if (!dateString) return 'Non spécifié';
    
    try {
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return 'Date invalide';
      
      return date.toLocaleDateString('fr-FR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      });
    } catch (error) {
      return 'Date invalide';
    }
  }

  // Télécharger un document
  downloadDocument(url: string): void {
    window.open(url, '_blank');
  }
}
