import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-patient-appointments',
  templateUrl: './patient-appointments.component.html',
  styleUrls: ['./patient-appointments.component.css']
})
export class PatientAppointmentsComponent implements OnInit {
  appointments: any[] = [];
  patientId: number | null = null;
  loading = false;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    const id = localStorage.getItem('patientId');
    if (!id) {
      this.router.navigate(['/login']);
      return;
    }
    this.patientId = parseInt(id, 10);
    this.loadAppointments();
  }

  loadAppointments() {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/appointments/patient/${this.patientId}`)
      .subscribe({
        next: (data) => {
          this.appointments = data;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
  }

  getStatusName(status: number) {
    switch (status) {
      case 0: return 'Agendado';
      case 1: return 'Em Espera';
      case 2: return 'Atendido';
      case 3: return 'Faltou';
      case 4: return 'Cancelado';
      default: return 'Desconhecido';
    }
  }

  getStatusClass(status: number) {
    switch (status) {
      case 0: return 'status-0';
      case 1: return 'status-1';
      case 2: return 'status-2';
      case 3: return 'status-3';
      case 4: return 'status-4';
      default: return '';
    }
  }

  formatTime(time: string) {
    if (!time) return '';
    const parts = time.split(':');
    return `${parts[0]}:${parts[1]}`;
  }

  logout() {
    localStorage.removeItem('patientId');
    this.router.navigate(['/login']);
  }
}
