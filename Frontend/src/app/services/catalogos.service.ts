import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LoggerService } from './logger.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CatalogosService {
  private readonly apiUrl = `${environment.apiUrl}/GenCatalogoDetalle`;

  constructor(
    private readonly http: HttpClient,
    private readonly logger: LoggerService
  ) {  }

   async getCatalogosPromise(rqConsultas: any): Promise<any> {
    const post$ = this.http.post<any>(`${this.apiUrl}/getallbynombrecatalogo`, rqConsultas);
    const result = await firstValueFrom(post$);
    return result;
  }
}
