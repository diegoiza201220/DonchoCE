import { Component, OnInit, inject } from '@angular/core';
import { AuthService } from '@services/auth.service';
import { BaseComponent } from '@util/base.component';
import { LoggerService } from '@services/logger.service';
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent extends BaseComponent implements OnInit {

  mostrarItems: boolean = false;
  mostrarReportes: boolean = false;
  
  public override authService = inject(AuthService);
  public override logger = inject(LoggerService);

  constructor(
  ) {
    super();
  }

  ngOnInit(): void {
   this.configureMenu();
  }

  logOut(){
    this.authService.logOut();
  }

  configureMenu(){
    this.mostrarItems = this.mostrarReportes = this.emailsPermitidos.indexOf(this.authService.userEmail) !== -1;
  }
}
