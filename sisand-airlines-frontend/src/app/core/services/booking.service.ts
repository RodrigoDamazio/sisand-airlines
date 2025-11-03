import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private baseUrl = `${environment.apiUrl}/bookings`;

  constructor(private http: HttpClient) {}

  createBooking(booking: any) {
    return this.http.post(this.baseUrl, booking);
  }

  getUserBookings(userId: string) {
    return this.http.get(`${this.baseUrl}/user/${userId}`);
  }
}
