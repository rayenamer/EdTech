import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { FormGroup, FormsModule } from '@angular/forms';
import { AdminService } from '../services/admin.service';
@Component({
  selector: 'app-admin-login',
  imports: [RouterModule, FormsModule, ToastrModule, RouterOutlet],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.css'
})
export class AdminLoginComponent {
    AdminService = inject(AdminService);
    private router = inject(Router);
    private toastr = inject(ToastrService);
    model : any = {};

    login(){ 
      this.AdminService.login(this.model).subscribe({
        next: _ => {
          console.log('API response:', Response);
          console.log(this.AdminService.currentUser()?.username);
          this.router.navigateByUrl('/AdminDashboard');
        },
        error: (error) => {
          const errorTitle = error?.error?.title;
          const message = errorTitle === 'Unauthorized' 
            ? 'Wrong email or password' 
            : errorTitle === 'Invalid username' ||'One or more validation errors occurred' 
            ? 'Please write your credentials' 
            : errorTitle || 'An error occurred';
          
          this.toastr.error(message);
        }
        
        
        
        
      })
    }
}
