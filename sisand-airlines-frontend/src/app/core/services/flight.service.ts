import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FlightService {
  private api = `${environment.API_URL}/voos`; // ✅ FICA ASSIM

  constructor(private http: HttpClient) {}

  getFlights(date: string): Observable<any[]> {
    const url = `${this.api}?data=${date}&minAssentos=1`;
    console.log('🌐 GET', url); // <-- deve mostrar /api/voos
    return this.http.get<any[]>(url);
  }

  getSeats(vooId: string): Observable<any[]> {
    const url = `${environment.API_URL}/assentos/${vooId}`; // ✅ CORRIGIDO
    console.log('🌐 GET', url);
    return this.http.get<any[]>(url);
  }
}

