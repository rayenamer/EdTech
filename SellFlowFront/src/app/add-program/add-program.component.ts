import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UniProgram } from '../models/UniProgram';
import { UniProgramService } from '../services/uni-program.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-program',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-program.component.html',
  styleUrl: './add-program.component.css'
})
export class AddProgramComponent {
  program: UniProgram = {
    id: 0,
    name: '',
    programStart: '',
    description: '',
    location: '',
    university: '',
    degree: '',
    duration: 0
  };

  constructor(
    private uniProgramService: UniProgramService,
    private router: Router
  ) {}

  onSubmit() {
    this.uniProgramService.addProgram(this.program).subscribe({
      next: () => {
        this.router.navigate(['/programs']);
      },
      error: (error) => {
        console.error('Error adding program:', error);
      }
    });
  }
}
