import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public userData: any;
  private readonly apiUrl = environment.apiUrl;

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router,
    private readonly ngZone: NgZone
  ) {}

  // log-in con email y contraseña contra la API REST
  logInWithEmailAndPassword(nombre: string, password: string): Promise<void> {
    return firstValueFrom(
      this.http.post<{ token: string; user: any }>(
        `${this.apiUrl}/login/validatelogin`,
        { nombre, password }
      )
    )
      .then((response) => {
        this.userData = response.user;
        localStorage.setItem('user', JSON.stringify(this.userData));
        localStorage.setItem('token', response.token);
        this.ngZone.run(() => this.router.navigate(['main']));
      })
      .catch((error) => {
        alert(error?.error?.message ?? 'Error al iniciar sesión');
      });
  }

  // return true when user is logged in
  get isLoggedIn(): boolean {
    const user = JSON.parse(localStorage.getItem('user')!);
    return user !== null;
  }

  get userEmail(): string {
    const user = JSON.parse(localStorage.getItem('user')!);
    return user ?? '';
  }

  // logOut
  logOut(): Promise<void> {
    return firstValueFrom(this.http.post<void>(`${this.apiUrl}/auth/logout`, {}))
      .finally(() => {
        localStorage.removeItem('user');
        localStorage.removeItem('token');
        this.router.navigate(['login']);
      });
  }
}
