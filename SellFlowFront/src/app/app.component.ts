import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule, RouterOutlet } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { AuthService } from './services/auth.service';
import { AdminService } from './services/admin.service';
import { NavbarComponent } from "./navbar/navbar.component";
@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterOutlet, RouterModule, NavbarComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  
  ngOnInit(): void {
    this.initializeUserState();
  }
  
  private authService = inject(AuthService);
  private adminService = inject(AdminService);
  
  private initializeUserState(): void {
    // Try to get current user from cookie-based authentication
    this.authService.getCurrentUser().subscribe({
      next: (user) => {
        this.authService.setCurrentUser(user);
      },
      error: () => {
        // If regular user auth fails, try admin auth
        this.adminService.getCurrentUser().subscribe({
          next: (admin) => {
            this.adminService.setCurrentUser(admin);
          },
          error: () => {
            // No valid authentication found, user is not logged in
            console.log('No valid authentication found');
          }
        });
      }
    });
  }

}
