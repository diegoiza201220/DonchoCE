import { Component } from '@angular/core';
import { BaseComponent } from 'src/app/util/base.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent extends BaseComponent {

  mostrarSucursales!: boolean;
  mostrarLogin: boolean = true;
  mostrarMensajeErrorSucursal: boolean = false;
  sucursales: any[] | undefined;
  selectedSucursal: string | undefined;

  logIn(usuario: string, password: string) {
    this.authService.logIn(usuario, password).then((response) => {
      if (this.authService.lSucursales.length === 0) {
        this.logger.log("No hay sucursales disponibles para este usuario");
        this.mostrarMensajeErrorSucursal = true;
        this.mostrarLogin = false;
      } else if (this.authService.lSucursales.length === 1) {
        this.logger.log("Hay una sucursal disponible para este usuario");
        this.authService.sucursalNombreSeleccionada = this.authService.lSucursales[0].sucursal.nombre;
        this.authService.sucursalIdSeleccionada = this.authService.lSucursales[0].sucursal.id;
        this.authService.setUserDataInLocalStorage();
        this.authService.routerNavigateMain();
      } else {
        this.logger.log("Hay varias sucursales disponibles para este usuario");
        this.mostrarSucursales = true;
        this.mostrarLogin = false;
        this.sucursales = this.authService.lSucursales.map((sucursal: any) => {
          return { nombre: sucursal.sucursal.nombre, code: sucursal.sucursal.id };
        });
      }
    }).catch((error) => {
      this.logger.log(" logInWithUserAndPassword error: " + error);
    });
  }

  onSucursalSelect(event: any) {
    this.authService.sucursalNombreSeleccionada = event.value.nombre;
    this.authService.sucursalIdSeleccionada = event.value.code;
    this.authService.setUserDataInLocalStorage();
    this.authService.routerNavigateMain();
  }
}
