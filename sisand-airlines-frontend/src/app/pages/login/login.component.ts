import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['login.component.scss']
})
export class LoginComponent {
  email = '';
  password = '';

  login() {
    console.log('Login:', this.email, this.password);
    alert('Login simulado com sucesso!');
  }
}
