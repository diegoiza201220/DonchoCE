import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ClientesService {
  private readonly apiUrl = `${environment.apiUrl}/cliente`;

  constructor(private readonly http: HttpClient) { }

  async addItem(cliente: any): Promise<any> {
    return firstValueFrom(this.http.post<any>(`${this.apiUrl}/crear`, cliente));
  }

  async getClientePromiseByCedulaRuc(cedulaRuc: string): Promise<any> {
    return firstValueFrom(this.http.get<any>(`${this.apiUrl}/cedula/${cedulaRuc}`));
  }

  async getClientesPromise(): Promise<any> {
    return firstValueFrom(this.http.get<any>(this.apiUrl));
  }

  async deleteCliente(cliente: any): Promise<any> {
      return firstValueFrom(this.http.delete(this.apiUrl + `/${cliente.id}`, cliente ));
    }
  
  async updateCliente(cliente: any): Promise<any> {
      return firstValueFrom(this.http.put(this.apiUrl + `/${cliente.id}`, cliente));
    }
}
