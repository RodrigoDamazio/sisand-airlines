import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
 // caminho relativo correto

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule],
  providers: [ApiService],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent {
  seats: any[] = [];
  loading = true;

  constructor(private api: ApiService) {
    this.api.getFlights().subscribe({
      next: (data) => {
        this.seats = data;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }
}
