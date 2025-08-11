import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  imports: [ReactiveFormsModule,RouterOutlet,FormsModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent implements OnInit{
  ForgotService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  ValidationErrors: string[] = [];
  ForgotPasswordForm: FormGroup = new FormGroup({});



  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.ForgotPasswordForm = new FormGroup({
      Email: new FormControl('',[Validators.required,Validators.email]),
    })
  }

  SendForgotPassword(){
    this.ForgotService.forgotPassword(this.ForgotPasswordForm.value).subscribe({
      next: _ => {
        console.log("successfully sent reset Password Email")
        this.toastr.info("a link was sent to your mail to reset your password")
      },
      error: (error) => {
        console.log(error)
      },
    });
  }



}
