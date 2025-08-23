import { Component, OnInit, HostListener, inject, computed } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AdminService } from '../services/admin.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterModule, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
  isMenuOpen = false;
  isScrolled = false;

  private adminService = inject(AdminService);
  private authService = inject(AuthService);

  isAdminLoggedIn = computed(() => this.adminService.isAuthenticated());
  isUserLoggedIn = computed(() => this.authService.isAuthenticated());

  ngOnInit() {
    this.checkScroll();
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    this.checkScroll();
  }

  checkScroll() {
    this.isScrolled = window.scrollY > 50;
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu() {
    this.isMenuOpen = false;
  }
  logout() {
    // Check if user is admin or regular user and logout accordingly
    //if (this.adminService.isAuthenticated()) {
    //  this.adminService.logout().subscribe({
    //    next: () => {
    //      console.log('Admin logged out successfully');
    //    },
    //    error: (error) => {
    //      console.error('Admin logout error:', error);
    //    }
    //  });
    //} else if (this.authService.isAuthenticated()) {
    //  this.authService.logout().subscribe({
    //    next: () => {
    //      console.log('User logged out successfully');
    //    },
    //    error: (error) => {
    //      console.error('User logout error:', error);
    //    }
    //  });
    //}
  }
}
