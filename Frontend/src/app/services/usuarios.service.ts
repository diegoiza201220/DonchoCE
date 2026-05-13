import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoggerService } from './logger.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsuariosService {
  private readonly apiUrl = `${environment.apiUrl}/usuario`;

  constructor(
    private readonly http: HttpClient,
    private readonly logger: LoggerService
  ) {}
}
