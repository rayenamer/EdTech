import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminService } from '../services/admin.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-userFoAdmin',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="admin-dashboard-container">
      <div class="admin-dashboard-card">
        <div class="admin-dashboard-header">
          <h2>Users</h2>
          <p>Complete list of platform users</p>
        </div>
        <div class="table-responsive">
          <table class="table table-bordered table-hover align-middle shadow-sm">
            <thead>
              <tr>
                <th>ID</th>
                <th>Username</th>
                <th>Email</th>
                <th>Gender</th>
                <th>City</th>
                <th>Roles</th>
                <th>LastActive</th>
              </tr>
            </thead>
            <tbody>
              @for (user of users; track user.id) {
                <tr>
                  <td>{{ user.id }}</td>
                  <td>{{ user.username }}</td>
                  <td>{{ user.email }}</td>
                  <td>{{ user.gender }}</td>
                  <td>{{ user.city }}</td>
                  <td>
                    @if (user.roles.length === 0) {
                      <span>User</span>
                    } @else {
                      @for (role of user.roles; track $index) {
                        <span class="badge bg-success me-1">{{ role }}</span>
                      }
                    }
                  </td>
                  <td>{{ user.lastActive }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class UserFoAdminComponent implements OnInit {
  private adminService = inject(AdminService);
  private toastr = inject(ToastrService);
  users: any[] = [];

  ngOnInit(): void {
    this.getUsers();
  }

  getUsers() {
    this.adminService.GetAllUsersForAdmin().subscribe({
      next: users => {
        this.users = users;
      },
      error: (error) => {
        const errorTitle = error?.error?.title;
        this.toastr.error(errorTitle || 'An error occurred while fetching users');
      }
    });
  }
}


