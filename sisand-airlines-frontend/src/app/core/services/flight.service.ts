import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FlightService {
  private baseUrl = `${environment.apiUrl}/flights`;

  constructor(private http: HttpClient) {}

  getFlights(date: string, passengers: number): Observable<any> {
    return this.http.get(`${this.baseUrl}?date=${date}&passengers=${passengers}`);
  }
}
