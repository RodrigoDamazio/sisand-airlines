import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlightService } from '../../core/services/flight.service';
import { BookingService } from '../../core/services/booking.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  flights: any[] = [];
  seats: any[] = [];
  selectedFlight: any = null;
  selectedSeat: any = null;
  message = '';
  loading = false; // ✅ <-- Adicionado

  constructor(
    private flightService: FlightService,
    private bookingService: BookingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const today = new Date().toISOString().split('T')[0];
    this.loading = true; // ✅ começa o loading
    this.flightService.getFlights(today).subscribe({
      next: (flights) => {
        this.flights = flights;
        this.loading = false; // ✅ terminou
      },
      error: (err) => {
        this.message = 'Erro ao carregar voos: ' + err.message;
        this.loading = false;
      }
    });
  }

  selectFlight(flight: any) {
    this.selectedFlight = flight;
    this.loading = true; // ✅ começa loading
    this.flightService.getSeats(flight.id).subscribe({
      next: (seats) => {
        this.seats = seats;
        this.loading = false;
      },
      error: (err) => {
        this.message = 'Erro ao carregar assentos: ' + err.message;
        this.loading = false;
      }
    });
  }

  selectSeat(seat: any) {
    this.selectedSeat = seat;
  }

  confirmarReserva() {
  console.log('✅ confirmarReserva() chamado');

  if (!this.selectedSeat || !this.selectedFlight) {
    console.warn('⚠️ Faltando voo ou assento:', {
      voo: this.selectedFlight,
      assento: this.selectedSeat
    });
    this.message = 'Selecione um voo e um assento.';
    return;
  }

  const usuarioId = this.authService.getUserId();
  console.log('👤 usuarioId obtido:', usuarioId);

  if (!usuarioId) {
    this.message = 'Usuário não autenticado. Faça login novamente.';
    console.warn('⚠️ UsuarioId null — token pode estar inválido ou ausente.');
    console.log('🔍 Token atual:', this.authService.getToken());
    return;
  }

  this.loading = true;

  const payload = {
    assentoId: this.selectedSeat.id,
    usuarioId
  };

  console.log('📦 Enviando payload de reserva:', payload);

  this.bookingService.reservar(payload).subscribe({
    next: () => {
      console.log('✅ Reserva enviada com sucesso');
      this.message = '✅ Reserva confirmada! Verifique seu e-mail.';
      this.loading = false;
    },
    error: (err) => {
      console.error('❌ Erro ao reservar:', err);
      this.message = 'Erro ao reservar: ' + (err.error?.message || err.message);
      this.loading = false;
    }
  });
}

}
