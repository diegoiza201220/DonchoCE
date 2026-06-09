import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import Orden from '../interfaces/orden.interface';
import { environment } from '../../environments/environment';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrdenesService {
  private readonly apiUrl = `${environment.apiUrl}/orden`;

  constructor(private readonly http: HttpClient) { }

  async addOrden(orden: any): Promise<any> {
    return firstValueFrom(this.http.post<any>(this.apiUrl + "/facturar", orden));
  }

  async deleteOrden(orden: Orden): Promise<any> {
    return firstValueFrom(this.http.delete<any>(`${this.apiUrl}/${orden.id}`));
  }

  async queryOrdenesPorFecha(rqOrdenesPorFechas: any): Promise<any> {
    const post$ = this.http.post<any>(`${this.apiUrl}/ordenesporFecha`, rqOrdenesPorFechas);
    const result = await firstValueFrom(post$);
    return result;
  }

  async queryProductosVendidosPorFecha(rqOrdenesPorFechas: any): Promise<any> {
    const post$ = this.http.post<any>(`${this.apiUrl}/productosvendidosporfecha`, rqOrdenesPorFechas);
    const result = await firstValueFrom(post$);
    return result;
  }

  async getDatosPedido (): Promise<any> {
    const post$ = this.http.get<any>(`${this.apiUrl}/datospedido`);
    const result = await firstValueFrom(post$);
    return result;
  }

  async queryFacturasPorFecha(rqFacturasPorFechas: any): Promise<any> {
    const post$ = this.http.post<any>(`${this.apiUrl}/facturasporfecha`, rqFacturasPorFechas);
    const result = await firstValueFrom(post$);
    return result;
  }
}
