import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { BaseComponent } from 'src/app/util/base.component';
import { LoggerService } from 'src/app/services/logger.service';
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent extends BaseComponent implements OnInit {

  mostrarItems: boolean = false;
  mostrarReportes: boolean = false;
  sucursalNombre: string = '';
 
  constructor(public override authService: AuthService, 
    public override logger: LoggerService
  ) {
    super(authService, logger);
  }

  ngOnInit(): void {
   this.configureMenu();
   this.mostrarSucursalNombre();
  }

  logOut(){
    this.authService.logOut();
  }

  configureMenu(){
    this.mostrarItems = this.mostrarReportes = this.emailsPermitidos.indexOf(this.authService.userEmail) !== -1;
  }

  mostrarSucursalNombre(): string {
    this.sucursalNombre = this.authService.getLocalStorageDataByKey('sucursalNombre')?.replaceAll(' ', '<br>') ?? '';
    return this.sucursalNombre;
  }
}
