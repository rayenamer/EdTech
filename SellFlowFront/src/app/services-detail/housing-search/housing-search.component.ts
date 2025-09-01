import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-housing-search',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './housing-search.component.html',
  styleUrl: './housing-search.component.css'
})
export class HousingSearchComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}