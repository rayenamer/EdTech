import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { RedirectCommand, Router, RouterModule } from '@angular/router';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { JsonPipe, NgIf } from '@angular/common';
@Component({
    standalone: true,
    selector: 'app-register',
    imports: [JsonPipe, ReactiveFormsModule, RouterModule, ToastrModule,NgIf],
    templateUrl: './register.component.html',
    styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  authService = inject(AuthService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  
  validationErrors: string[] = [];
  model: any = {};
  registerForm: FormGroup = new FormGroup({});
  isLoading = false;
  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = new FormGroup({
      Gender: new FormControl('male'),
      Username: new FormControl ('', Validators.required),
      city: new FormControl ('', Validators.required),
      Country: new FormControl ('', Validators.required),
      PhoneNumber: new FormControl ('', Validators.required), // ✅ Add this
      Email: new FormControl ('',[ Validators.required,Validators.email]),
      Password: new FormControl( '', [
        Validators.required,
        Validators.minLength(6),
        this.passwordRequiresDigit(),
        this.passwordRequiresLower(),
        this.passwordRequiresUpper(),
        this.passwordRequiresUniqueChars(),
      ])
    });

  }
  passwordRequiresDigit(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const hasDigit = /[0-9]/.test(control.value);
      return hasDigit ? null : { passwordRequiresDigit: true };
    };
  }
  
  passwordRequiresLower(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const hasLower = /[a-z]/.test(control.value);
      return hasLower ? null : { passwordRequiresLower: true };
    };
  }
  
  passwordRequiresUpper(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const hasUpper = /[A-Z]/.test(control.value);
      return hasUpper ? null : { passwordRequiresUpper: true };
    };
  }
  
  passwordRequiresUniqueChars(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(control.value);
      return hasSpecial ? null : { passwordRequiresUniqueChars: true };
    };
  }
  

  register() {
    this.authService.register(this.registerForm.value).subscribe({
      next: _ => {

        console.log("success")
        this.toastr.info("a link was sent to your mail to confirm your identity")
        this.router.navigateByUrl('/login');
      },
      error: (error) => {
        
    },
    
    });
  }

}







