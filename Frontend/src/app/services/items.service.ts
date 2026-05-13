import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import Item from '../interfaces/item.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ItemsService {
  private readonly apiUrl = `${environment.apiUrl}/item`;

  constructor(private readonly http: HttpClient) {}

  addItem(item: Item): Observable<Item> {
    return this.http.post<Item>(this.apiUrl, item);
  }

  getItemsPromise(): Promise<Item[]> {
    return firstValueFrom(this.http.get<Item[]>(this.apiUrl));
  }

  getItemsObservable(): Observable<Item[]> {
    return this.http.get<Item[]>(this.apiUrl);
  }

  deleteItem(item: Item): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${item.id}`);
  }

  updateItem(item: Item): Observable<Item> {
    return this.http.put<Item>(`${this.apiUrl}/${item.id}`, item);
  }
}
