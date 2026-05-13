import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, interval, switchMap, startWith, share } from 'rxjs';
import Produccioncocina from '../interfaces/produccioncocina.interface';
import { LoggerService } from './logger.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProduccioncocinaService {
  private readonly apiUrl = `${environment.apiUrl}/produccioncocina`;

  // Polling cada 5 segundos para simular el listener en tiempo real de Firestore
  private readonly produccioncocina$: Observable<Produccioncocina[]> = interval(5000).pipe(
    startWith(0),
    switchMap(() => this.http.get<Produccioncocina[]>(this.apiUrl)),
    share()
  );

  constructor(
    private readonly http: HttpClient,
    private readonly logger: LoggerService
  ) {}

  getProduccioncocinaObservable(): Observable<Produccioncocina[]> {
    return this.produccioncocina$;
  }

  updateProduccioncocina(produccionCocina: Produccioncocina): Observable<Produccioncocina> {
    this.logger.log(produccionCocina);
    return this.http.put<Produccioncocina>(
      `${this.apiUrl}/${produccionCocina.id}`,
      produccionCocina
    );
  }
}
