import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-interview-preparation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './interview-preparation.component.html',
  styleUrl: './interview-preparation.component.css'
})
export class InterviewPreparationComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}