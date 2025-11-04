import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'   // garante que o serviço exista em runtime
})
export class ApiService {
  private readonly baseUrl = 'http://localhost:5300/api'; // ajuste se precisar

  constructor(private http: HttpClient) {}

  getFlights(): Observable<any> {
    return this.http.get(`${this.baseUrl}/flights`);
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/auth/login`, { email, password });
  }
}
