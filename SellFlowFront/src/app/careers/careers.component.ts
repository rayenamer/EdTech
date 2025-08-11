import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-careers',
    imports: [RouterOutlet, RouterModule],
    templateUrl: './careers.component.html',
    styleUrl: './careers.component.css'
})
export class CareersComponent {

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
