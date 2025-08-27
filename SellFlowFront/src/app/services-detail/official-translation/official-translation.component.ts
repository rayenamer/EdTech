import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-official-translation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './official-translation.component.html',
  styleUrl: './official-translation.component.css'
})
export class OfficialTranslationComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}