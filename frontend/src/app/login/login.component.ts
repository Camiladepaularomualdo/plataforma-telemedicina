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
  profileType: 'doctor' | 'patient' = 'doctor';
  isLoginMode = true;

  // Doctor Login fields
  loginEmail = '';
  loginPassword = '';
  loginError = '';

  // Patient Login fields
  isFirstAccess = false;
  patientEmail = '';
  patientPassword = '';
  successMsg = '';

  // Doctor Register fields
  regName = '';
  regEmail = '';
  regPassword = '';
  regCpf = '';
  regPhone = '';
  regBirthDate = '';
  regError = '';

  constructor(private http: HttpClient, private router: Router) { }

  setProfile(profile: 'doctor' | 'patient') {
    this.profileType = profile;
    this.loginError = '';
    this.regError = '';
    this.successMsg = '';
  }

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    this.loginError = '';
    this.regError = '';
  }

  togglePatientMode() {
    this.isFirstAccess = !this.isFirstAccess;
    this.loginError = '';
    this.successMsg = '';
  }

  onLogin() {
    this.loginError = '';
    const payload = { email: this.loginEmail, password: this.loginPassword };

    this.http.post<any>(`${environment.apiUrl}/auth/login`, payload).subscribe({
      next: (res) => {
        localStorage.setItem('doctorId', res.id);
        localStorage.setItem('doctorRule', res.rule || 'usr');
        this.router.navigate(['/agenda']);
      },
      error: (err) => {
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
      },
      error: (err) => {
        this.regError = err.error || 'Registration failed';
      }
    });
  }

  onPatientLogin() {
    this.loginError = '';
    this.successMsg = '';
    const payload = { email: this.patientEmail, password: this.patientPassword };

    this.http.post<any>(`${environment.apiUrl}/auth/patient/login`, payload).subscribe({
      next: (res) => {
        localStorage.setItem('patientId', res.id);
        this.router.navigate(['/meus-agendamentos']);
      },
      error: (err) => {
        this.loginError = err.error || 'Email ou senha inválidos';
      }
    });
  }

  onPatientFirstAccess() {
    this.loginError = '';
    this.successMsg = '';
    const payload = { email: this.patientEmail };

    this.http.post<any>(`${environment.apiUrl}/auth/patient/first-access`, payload).subscribe({
      next: (res) => {
        this.successMsg = res.message;
        this.isFirstAccess = false;
        this.patientPassword = '';
      },
      error: (err) => {
        this.loginError = err.error || 'Paciente não encontrado';
      }
    });
  }
}
