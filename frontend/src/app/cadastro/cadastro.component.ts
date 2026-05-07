import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-cadastro',
  templateUrl: './cadastro.component.html',
  styleUrls: ['./cadastro.component.css']
})
export class CadastroComponent {
  regName = '';
  regEmail = '';
  regPassword = '';
  regCpf = '';
  regPhone = '';
  regBirthDate = '';
  regError = '';

  constructor(private http: HttpClient, private router: Router) { }

  onRegister() {
    this.regError = '';
    const payload = {
      name: this.regName,
      email: this.regEmail,
      passwordHash: this.regPassword,
      cpf: this.regCpf,
      phone: this.regPhone,
      birthDate: this.regBirthDate
    };

    this.http.post<any>(`${environment.apiUrl}/auth/register`, payload).subscribe({
      next: (res) => {
        // Auto-login upon successful registration
        localStorage.setItem('doctorId', res.id);
        localStorage.setItem('doctorRule', res.rule || 'usr');
        this.router.navigate(['/agenda']);
      },
      error: (err) => {
        this.regError = err.error || 'Falha ao criar conta. Verifique os dados e tente novamente.';
      }
    });
  }
}
