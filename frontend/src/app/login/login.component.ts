import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  isLoginMode = true;

  // Login fields
  loginEmail = '';
  loginPassword = '';
  loginError = '';

  // Register fields
  regName = '';
  regEmail = '';
  regPassword = '';
  regCpf = '';
  regPhone = '';
  regBirthDate = '';
  regError = '';

  constructor(private http: HttpClient, private router: Router) { }

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    this.loginError = '';
    this.regError = '';
  }

  onLogin() {
    this.loginError = '';
    const payload = { email: this.loginEmail, password: this.loginPassword };
    // The exact port depends on the user environment. Assuming http://localhost:5249 or similar.
    // I will use relative or proxy, or simply http://localhost:5000 / https://localhost:5001. 
    // Usually .NET 9 Web API creates random ports. I'll read the launchSettings.json next to fix this.

    // For now:
    this.http.post<any>(`${environment.apiUrl}/auth/login`, payload).subscribe({
      next: (res) => {
        localStorage.setItem('doctorId', res.id);
        localStorage.setItem('doctorRule', res.rule || 'usr');
        this.router.navigate(['/agenda']);
      },
      error: (err) => {
        // Exibe a mensagem de erro específica do backend se houver (ex: conta inativa), ou uma padrão
        this.loginError = err.error || 'Email ou senha inválidos';
      }
    });
  }

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
        this.loginEmail = this.regEmail;
        this.loginPassword = this.regPassword;
        this.toggleMode();
        // optionally auto login
      },
      error: (err) => {
        this.regError = err.error || 'Registration failed';
      }
    });
  }
}
