import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

type LoginRequest = { email: string; senha: string }; // backend espera "Senha"
type LoginResponse = { token: string; usuarioId: string };

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly base = environment.API_URL;

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, senha: string): Observable<LoginResponse> {
  return this.http.post<LoginResponse>(`${this.base}/auth/login`, { email, senha } as LoginRequest)
    .pipe(
      tap((res: any) => {
        if (res?.token) {
          localStorage.setItem('token', res.token);

          // ✅ cobre casos onde backend usa "usuarioId" OU "userId" OU "id"
          const userId = res.usuarioId || res.userId || res.id;
          if (userId) localStorage.setItem('usuarioId', userId);
        } else {
          console.error('⚠️ Login response não contém token JWT:', res);
        }
      })
    );
}


  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('usuarioId');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserId(): string | null {
  const token = this.getToken();
  if (!token) return null;

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));

    return (
      payload?.sub ||
      payload?.nameid ||
      payload?.userId ||
      payload?.usuarioId ||
      null
    );
  } catch (error) {
    console.warn('Erro ao decodificar token JWT:', error);
    return null;
  }
}

}
