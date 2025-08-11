import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  imports: [NgIf,ReactiveFormsModule],
})
export class ResetPasswordComponent implements OnInit {
  formreset : FormGroup = new FormGroup({});;
  token!: string;
  email!: string;
  message: string = '';
  error: string = '';

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('code') || '';
    this.email = this.route.snapshot.queryParamMap.get('email') || '';

    this.formreset = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.formreset.invalid) return;

    const payload = {
      Email: this.email,
      Token: this.token,
      Password: this.formreset.value.password
    };

    this.http.post('http://localhost:5161/api/Register_Login/ResetPassword', payload)
      .subscribe({
        next: (res: any) => {
          this.message = 'Password has been reset successfully!';
          this.router.navigateByUrl('/login');
        },
        error: err => {
          console.log(err);
        }
      });
  }
}
