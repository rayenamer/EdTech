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
      this.authService.logout();
      window.location.href = '/';
    });
  }

  
  private handleGoogleRedirect(): void {
    this.route.queryParamMap.subscribe(params => {
      const status = params.get('status');
      if (status === 'success') {
        this.authService.getCurrentUser().subscribe({
          next: user => {
            this.authService.setCurrentUser(user);
            this.router.navigate(['/dashboard']);
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
