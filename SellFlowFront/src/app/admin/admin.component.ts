import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-admin',
  imports: [],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit{
  totalUsers = 128;
  totalPrograms = 24;
  totalApplications = 312;

  ngOnInit(): void {
    // Static dashboard for now
  }
}
