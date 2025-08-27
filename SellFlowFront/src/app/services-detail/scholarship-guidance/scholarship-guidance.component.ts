import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-scholarship-guidance',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './scholarship-guidance.component.html',
  styleUrl: './scholarship-guidance.component.css'
})
export class ScholarshipGuidanceComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}