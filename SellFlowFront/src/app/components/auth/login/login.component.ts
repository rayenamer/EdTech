import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { ActivatedRoute, Router, RouterModule, RouterOutlet } from '@angular/router';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { FormGroup, FormsModule } from '@angular/forms';

@Component({
    selector: 'app-login',
    imports: [RouterModule, FormsModule, ToastrModule, RouterOutlet], // ✅ Ensure ToastrModule is imported
    templateUrl: './login.component.html',
    styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  ngOnInit(): void {
   this.handleGoogleRedirect();
  }
 
  private route = inject(ActivatedRoute);
  authService = inject(AuthService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  isLoading = false;
  model : any = {};


  login(){ 
    this.authService.login(this.model).subscribe({
      next: _ => {
        console.log('API response:', Response);
        console.log(this.authService.currentUser()?.username);
        this.isLoading = true;
        this.router.navigateByUrl('/Acceuil');
      },
      error: (error) => {
        const errorTitle = error?.error?.title;
        const message = errorTitle === 'Unauthorized' 
          ? 'Wrong email or password' 
          : errorTitle === 'Invalid username' ||'One or more validation errors occurred' 
          ? 'Please write your credentials' 
          : errorTitle || 'An error occurred';
        
        this.toastr.error(message);
        this.isLoading = false;
      }
      
      
      
      
    })
  }

  loginWithGoogle() {
    window.location.href = 'https://localhost:7030/api/Register_Login/google-login';
  }

  logout() {
    // Call backend to sign out (Google + cookies)
    fetch('https://localhost:7030/api/Register_Login/google-signout', {
      method: 'POST',
      credentials: 'include'
    }).then(() => {
      this.authService.logout().subscribe({
        next: () => {
          window.location.href = '/';
        },
        error: (error) => {
          console.error('Logout error:', error);
          window.location.href = '/';
        }
      });
    });
  }

  
  private handleGoogleRedirect(): void {
    this.route.queryParamMap.subscribe(params => {
      const status = params.get('status');
      const userDataEncoded = params.get('userData');
      
      if (status === 'success' && userDataEncoded) {
        try {
          // Decode the base64 user data
          const userDataJson = atob(userDataEncoded);
          const userData = JSON.parse(userDataJson);
          
          // Create user object matching your User interface
          const user = {
            username: userData.username,
            token: userData.token,
            gender: userData.gender,
            email: userData.email,
            city: userData.city,
            country: userData.country,
            phoneNumber: userData.phoneNumber,
            emailConfirmed: userData.emailConfirmed,
            password: '' // Google users don't have passwords
          };
          
          // Set current user (no localStorage needed with cookies)
          this.authService.currentUser.set(user);
          
          // Navigate to home page
          this.router.navigate(['/Acceuil']);
          
        } catch (error) {
          console.error('Error parsing user data:', error);
          this.toastr.error('Failed to process Google login data.');
          this.router.navigate(['/login']);
        }
      } else if (status === 'success') {
        // Fallback: try to get user from backend if no userData in URL
        this.authService.getCurrentUser().subscribe({
          next: user => {
            this.authService.currentUser.set(user);
            this.router.navigate(['/Acceuil']);
          },
          error: () => {
            this.toastr.error('Failed to fetch user info after Google login.');
            this.router.navigate(['/login']);
          }
        });
      }
    });
  }
}
