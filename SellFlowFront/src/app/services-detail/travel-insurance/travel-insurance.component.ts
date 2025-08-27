import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-travel-insurance',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './travel-insurance.component.html',
  styleUrl: './travel-insurance.component.css'
})
export class TravelInsuranceComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}