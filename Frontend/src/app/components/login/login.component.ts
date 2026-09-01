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

  logIn(usuario: string, password: string) {
    this.mostrarSucursales = true;
    this.authService.logIn(usuario, password).then((response) => {
      // this.logger.log("logIn response: " + localStorage.getItem('token'));
      this.logger.log("logIn response: " + this.authService.user);
      if (this.authService.lSucursales.length === 0) {
        this.logger.log("No hay sucursales disponibles para este usuario");
      } else if (this.authService.lSucursales.length === 1) {
        this.logger.log("Hay una sucursal disponible para este usuario");
      } else {
        this.logger.log("Hay varias sucursales disponibles para este usuario");
      }
    });

    //const token = localStorage.getItem('token');
    // .then((response)=>{
    //   this.logger.log(response.sucursales.count);
    // }.catch((error)=>{
    //   this.logger.log(" logInWithUserAndPassword error: " + error);
    // }

    // )
  }
}