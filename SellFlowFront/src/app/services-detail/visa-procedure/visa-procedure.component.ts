import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-visa-procedure',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './visa-procedure.component.html',
  styleUrl: './visa-procedure.component.css'
})
export class VisaProcedureComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome() {
    this.router.navigate(['/']);
  }
}