import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private baseUrl = `${environment.API_URL}`

  constructor(private http: HttpClient) {}

    reservar(payload: { assentoId: string; usuarioId: string }): Observable<any> {
      return this.http.post(`${this.baseUrl}/checkout/reservar`, payload);
    }

    getMinhasReservas() {
      return this.http.get<any[]>(`${this.baseUrl}/reservas/minhas`);
    }

    cancelarReserva(id: string) {
      return this.http.delete(`${this.baseUrl}/reservas/${id}`);
    }

}
