import { Injectable } from '@angular/core';
import { HttpClient, HttpParams  } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { tap } from 'rxjs/operators';
import Secuencia from '../interfaces/secuencia.interface';
import { LoggerService } from './logger.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SecuenciaService {
  private readonly apiUrl = `${environment.apiUrl}/FacSecuenciaDia`;

  constructor(
    private readonly http: HttpClient,
    private readonly logger: LoggerService
  ) { }

  getSecuenciaObservable(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl)
      .pipe(tap(data => this.logger.log(data)));
  }

  updateSecuencia(secuencia: Secuencia): Observable<Secuencia> {
    return this.http.put<Secuencia>(`${this.apiUrl}/${secuencia.id}`, secuencia);
  }

  getSecuenciaPromise(id: string): Promise<any> {
    //const params = new HttpParams().set('id', id);
    return firstValueFrom(this.http.get<any>(`${this.apiUrl}/${id}`)
      .pipe(tap(data => this.logger.log(data))));
  }
}
