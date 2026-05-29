import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
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

  // getItemsObservable(): Observable<Item[]> {
  //   return this.http.get<Item[]>(this.apiUrl);
  // }

  // deleteItem(item: Item): Observable<void> {
  //   return this.http.delete<void>(`${this.apiUrl}/${item.id}`);
  // }

  // updateItem(item: Item): Observable<Item> {
  //   return this.http.put<Item>(`${this.apiUrl}/${item.id}`, item);
  // }
}
