import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cadastro.component.html',
  styleUrls: ['./cadastro.component.scss']
})
export class CadastroComponent {
  nome = '';
  email = '';
  cpf = '';
  dataNascimento = '';
  senha = '';
  confirmarSenha = '';

  loading = false;
  message = '';

  constructor(private authService: AuthService, private router: Router) {}

  cadastrar() {
    if (!this.nome || !this.email || !this.cpf || !this.dataNascimento || !this.senha) {
      this.message = 'Preencha todos os campos obrigatórios.';
      return;
    }

    if (this.senha !== this.confirmarSenha) {
      this.message = 'As senhas não coincidem.';
      return;
    }

    // montar payload conforme backend espera
    const payload = {
      nomeCompleto: this.nome,
      email: this.email,
      cpf: this.cpf,
      dataNascimento: this.dataNascimento,
      senha: this.senha,
      confirmarSenha: this.confirmarSenha
    };

    this.loading = true;
    this.message = '';

    this.authService.register(payload).subscribe({
      next: (res) => {
        this.loading = false;
        // ajustar comportamento: por enunciado redireciona para checkout após cadastro
        this.message = 'Cadastro realizado com sucesso! Redirecionando...';
        // salvar automaticamente token se o backend retornar
        // por segurança, esperar um tempo curto para mostrar mensagem
        setTimeout(() => this.router.navigate(['/checkout']), 800);
      },
      error: (err) => {
        this.loading = false;
        this.message = 'Erro ao cadastrar: ' + (err?.error?.erro || err?.message || 'Erro desconhecido');
      }
    });
  }

  irParaLogin() {
    this.router.navigate(['/login']);
  }
}
