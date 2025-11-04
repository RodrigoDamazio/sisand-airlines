import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { BookingService } from '../../core/services/booking.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-minhas-reservas',
  standalone: true,
  imports: [CommonModule, DatePipe, CurrencyPipe],
  templateUrl: './minhas_reservas.component.html',
  styleUrls: ['./minhas_reservas.component.scss']
})
export class MinhasReservasComponent implements OnInit {
  reservas: any[] = [];
  loading = false;
  message = '';

  constructor(
    private bookingService: BookingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.carregarReservas();
  }

  carregarReservas() {
    this.loading = true;
    this.bookingService.getMinhasReservas().subscribe({
      next: (data) => {
        this.reservas = data;
        this.loading = false;
      },
      error: (err) => {
        this.message = 'Erro ao carregar reservas: ' + err.message;
        this.loading = false;
      }
    });
  }

  cancelarReserva(id: string) {
    if (!confirm('Tem certeza que deseja cancelar esta reserva?')) return;
    this.loading = true;
    this.bookingService.cancelarReserva(id).subscribe({
      next: () => {
        this.message = 'Reserva cancelada com sucesso!';
        this.carregarReservas();
      },
      error: (err) => {
        this.message = 'Erro ao cancelar: ' + err.message;
        this.loading = false;
      }
    });
  }
}
