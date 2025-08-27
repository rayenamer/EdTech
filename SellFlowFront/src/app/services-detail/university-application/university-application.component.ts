import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-university-application',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './university-application.component.html',
  styleUrl: './university-application.component.css'
})
export class UniversityApplicationComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome() {
    this.router.navigate(['/']);
  }
}