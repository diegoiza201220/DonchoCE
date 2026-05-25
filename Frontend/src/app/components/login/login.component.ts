import { Component } from '@angular/core';
import { BaseComponent } from '@util/base.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent extends BaseComponent {

  logIn(email: string, password: string) {
    return this.authService.logInWithEmailAndPassword(email, password)
      .then((user)=>{
        this.logger.log(" logInWithEmailAndPassword oK");
      }
    )
  }
}