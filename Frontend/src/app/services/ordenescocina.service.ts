import { Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, interval, switchMap, startWith, share } from 'rxjs';
import Ordencocina from '../interfaces/ordencocina.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrdenescocinaService {
  private readonly apiUrl = `${environment.apiUrl}/ordencocina`;

  // Polling cada 5 segundos para simular el listener en tiempo real de Firestore
  private readonly ordenescocina$: Observable<Ordencocina[]> = interval(5000).pipe(
    startWith(0),
    switchMap(() => this.http.get<Ordencocina[]>(this.apiUrl)),
    share()
  );

  constructor(private readonly http: HttpClient) {}

  addOrdencocina(ordenCocina: Ordencocina): Observable<Ordencocina> {
    return this.http.post<Ordencocina>(this.apiUrl, ordenCocina);
  }

  getOrdenescocinaObservable(): Observable<Ordencocina[]> {
    return this.ordenescocina$;
  }

  updateOrdenescocina(ordenCocina: Ordencocina): Observable<Ordencocina> {
    return this.http.put<Ordencocina>(`${this.apiUrl}/${ordenCocina.id}`, ordenCocina);
  }
}
