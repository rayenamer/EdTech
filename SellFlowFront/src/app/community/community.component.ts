import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-community',
  imports: [RouterOutlet, RouterModule],
  templateUrl: './community.component.html',
  styleUrl: './community.component.css'
})
export class CommunityComponent {
  // Method to handle Discord join action
  joinDiscord() {
    // Add Discord invite link here
    window.open('https://discord.gg/your-invite-link', '_blank');
  }
}
