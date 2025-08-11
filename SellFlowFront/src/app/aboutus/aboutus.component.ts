import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-aboutus',
    imports: [RouterOutlet, RouterModule],
    templateUrl: './aboutus.component.html',
    styleUrl: './aboutus.component.css'
})
export class AboutusComponent {

    scrollCards(section: string, direction: 'left' | 'right') {
        const cardsContainer = document.getElementById(`${section}-cards`);
        if (cardsContainer) {
            const scrollAmount = 350; // Adjust based on card width + gap
            const currentScroll = cardsContainer.scrollLeft;
            
            if (direction === 'left') {
                cardsContainer.scrollTo({
                    left: currentScroll - scrollAmount,
                    behavior: 'smooth'
                });
            } else {
                cardsContainer.scrollTo({
                    left: currentScroll + scrollAmount,
                    behavior: 'smooth'
                });
            }
        }
    }
}
