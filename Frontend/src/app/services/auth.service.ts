import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public user: any;
  public token: string = '';
  public lSucursales: any;
  public sucursalIdSeleccionada: number = 0;
  public sucursalNombreSeleccionada: string = '';
  private readonly apiUrl = environment.apiUrl;

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router,
    private readonly ngZone: NgZone
  ) { }


  // log-in con email y contraseña contra la API REST
  logIn(nombre: string, password: string): Promise<void> {
    return firstValueFrom(
      this.http.post<{ token: string; user: any; sucursales: any }>(
        `${this.apiUrl}/login/validatelogin`,
        { nombre, password }
      ))
      .then((response) => {
        this.user = JSON.stringify(response.user);
        this.token = response.token;
        this.lSucursales = response.sucursales;
      })
      .catch((error) => {
        alert(error?.error?.message ?? 'Error al iniciar sesión');
      });
  }

  routerNavigateMain(): void {
    this.ngZone.run(() => this.router.navigate(['main']));
  }

  setUserDataInLocalStorage(): void {
    localStorage.setItem('user', this.user);
    localStorage.setItem('token', this.token);
    localStorage.setItem('sucursalId', this.sucursalIdSeleccionada.toString());
    localStorage.setItem('sucursalNombre', this.sucursalNombreSeleccionada);
  }

  // return true when user is logged in
  get isLoggedIn(): boolean {
    // if (this.router.url === '/' || this.router.url === '/login' || this.router.url === undefined) {
    //   localStorage.removeItem('user');
    //   return false;
    // }
    //localStorage.removeItem('user');
    const user = JSON.parse(localStorage.getItem('user')!);
    return user !== null;
  }

  get userEmail(): string {
    const user = JSON.parse(localStorage.getItem('user')!);
    return user ?? '';
  }

  getLocalStorageDataByKey(key: string): string | null {
    return localStorage.getItem(key);
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
