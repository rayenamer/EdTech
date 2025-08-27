import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-hotel-ticket-reservation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hotel-ticket-reservation.component.html',
  styleUrl: './hotel-ticket-reservation.component.css'
})
export class HotelTicketReservationComponent {
  constructor(private router: Router) {}

  goBack() {
    this.router.navigate(['/']);
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}