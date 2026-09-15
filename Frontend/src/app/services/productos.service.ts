import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { tap } from 'rxjs/operators';
import Producto from '../interfaces/productos.interface';
import { LoggerService } from './logger.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductosService {
  private readonly apiUrl = `${environment.apiUrl}/producto`;

  constructor(
    private readonly http: HttpClient,
    private readonly logger: LoggerService
  ) { }

  async addProducto(producto: Producto): Promise<any> {
    const post$ = this.http.post<any>(`${this.apiUrl}/crear`, producto);
    const result = await firstValueFrom(post$);
    return result;
  }

  getProductosPromise(): Promise<any> {
    return firstValueFrom(this.http.get<any>(this.apiUrl)
      .pipe(tap(data => this.logger.log(data))));
  }

  getProductosObservable(): Observable<Producto[]> {
    return this.http.get<Producto[]>(this.apiUrl)
      .pipe(tap(data => this.logger.log(data)));
  }

  // deleteProducto(producto: Producto): Observable<any> {
  //   return this.http.delete(this.apiUrl + "/eliminar", { body: producto });
  // }

  async deleteProducto(producto: Producto): Promise<any> {
    const post$ = this.http.post<any>(`${this.apiUrl}/eliminar`, producto);
    const result = await firstValueFrom(post$);
    return result;
  }

  async updateProducto(producto: Producto): Promise<any> {
    const post$ = this.http.post<any>(`${this.apiUrl}/actualizar`, producto);
    const result = await firstValueFrom(post$);
    return result;
  }
}
