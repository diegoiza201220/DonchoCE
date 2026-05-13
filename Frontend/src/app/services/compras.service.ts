import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import Compra from '../interfaces/compra.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ComprasService {
  private readonly apiUrl = `${environment.apiUrl}/compra`;

  constructor(private readonly http: HttpClient) {}

  addCompra(compra: Compra): Observable<Compra> {
    return this.http.post<Compra>(this.apiUrl, compra);
  }

  async queryComprasPorFecha(d1: number, d2: number): Promise<Compra[]> {
    const params = new HttpParams()
      .set('fechaDesde', d1.toString())
      .set('fechaHasta', d2.toString());
    const compras = await firstValueFrom(this.http.get<Compra[]>(`${this.apiUrl}/porFecha`, { params }));
    const result = compras ?? [];
    result.sort((a, b) => (a.secuencial < b.secuencial ? -1 : 1));
    return result;
  }
}
