import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-careers',
    imports: [RouterOutlet, RouterModule, CommonModule],
    templateUrl: './careers.component.html',
    styleUrl: './careers.component.css'
})
export class CareersComponent {

    lifeContent = [
        {
            title: 'Global Diplomacy',
            description: 'Strong worldwide partnerships that foster cultural exchange and educational progress.',
            image: 'https://ik.imagekit.io/p51pyfdvv/mootaz%20with%20the%20president.jpg?updatedAt=1756104717738'
        },
        {
            title: 'Collaborative Teams',
            description: 'Brilliant minds thrive in our diverse, innovative environment.',
            image: 'https://ik.imagekit.io/p51pyfdvv/DSC01749.JPG?updatedAt=1756105175531'
        },
        {
            title: 'Healthy Work Environment',
            description: 'We enjoy our work and have fun together.',
            image: 'https://ik.imagekit.io/p51pyfdvv/0D2A9637.JPG?updatedAt=1756106533029'
        },
        {
            title: 'Career Growth',
            description: 'Grow through continuous learning, mentorship, and unique special missions.',
            image: 'https://ik.imagekit.io/p51pyfdvv/WhatsApp%20Image%202025-08-22%20at%2017.08.45.jpeg?updatedAt=1756106006226'
        },
        {
            title: 'Youth Leadership',
            description: 'Strong connections with young minds and emerging leaders worldwide.',
            image: 'https://ik.imagekit.io/p51pyfdvv/0D2A9632.JPG?updatedAt=1756104670315'
        },
        {
            title: 'Digital Influence',
            description: 'Strategic partnerships with top influencers amplify our educational mission.',
            image: 'https://ik.imagekit.io/p51pyfdvv/0D2A9592.JPG?updatedAt=1756106360957'
        },
        {
            title: 'Growth & Networking',
            description: 'Discover opportunities and connect with like-minded professionals.',
            image: 'https://ik.imagekit.io/p51pyfdvv/WhatsApp%20Image%202025-08-22%20at%2017.00.28.jpeg?updatedAt=1756104751188'
        },

    ];

    currentLifeIndex = 0;

    // Method to switch life content
    switchLifeContent(index: number) {
        this.currentLifeIndex = index;
    }

    // Method to handle form submission
    onSubmit(event: Event) {
        event.preventDefault();
        // Handle form submission logic here
        console.log('Talent community form submitted');
    }

    // Method to handle smooth scrolling to sections
    scrollToSection(sectionId: string) {
        const element = document.getElementById(sectionId);
        if (element) {
            element.scrollIntoView({ behavior: 'smooth' });
        }
    }
}
