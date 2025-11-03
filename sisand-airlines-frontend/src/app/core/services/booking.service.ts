import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private baseUrl = `${environment.API_URL}/checkout`;

  constructor(private http: HttpClient) {}

  reservar(payload: { assentoId: string; usuarioId: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/reservar`, payload);
  }
}
