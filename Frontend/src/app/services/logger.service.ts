import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LoggerService {
  log(message: any) {
    if (!this.isProd()) {
      console.log(message);
    }
  }

  private isProd(): boolean {
    return environment.production;
  }
}
