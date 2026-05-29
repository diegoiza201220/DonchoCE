import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig } from 'primeng/api';
import { BaseComponent } from './util/base.component';
import { AuthService } from './services/auth.service';
import { LoggerService } from './services/logger.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class AppComponent extends BaseComponent implements OnInit {

  showNavBar!: boolean;

  constructor(private primengConfig: PrimeNGConfig,
    public override authService: AuthService,
    public override logger: LoggerService,
    private readonly messageService: MessageService,
  ) {
    super(authService, logger);
  }

  ngOnInit() {
    this.primengConfig.ripple = true;
    this.showNavBar = this.isloggedIn;
  }

  title = 'app_doncho';


}
