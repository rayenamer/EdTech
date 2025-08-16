import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule, RouterOutlet } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { AuthService } from './services/auth.service';
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
  }
  
  private authService = inject(AuthService);
  

}
