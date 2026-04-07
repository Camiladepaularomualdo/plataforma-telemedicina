import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-agenda',
  templateUrl: './agenda.component.html',
  styleUrls: ['./agenda.component.css']
})
export class AgendaComponent implements OnInit {
  currentDate = new Date();
  appointments: any[] = [];
  calendarWeeks: any[][] = [];
  weekDays = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];
  selectedAppointment: any = null;
  doctorId: number | null = null;
  loading = false;

  showPatientForm = false;
  showAppointmentForm = false;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    const id = localStorage.getItem('doctorId');
    if (!id) {
      this.router.navigate(['/login']);
      return;
    }
    this.doctorId = parseInt(id, 10);
    this.loadAppointments();
  }

  get monthName(): string {
    return this.currentDate.toLocaleString('pt-BR', { month: 'long', year: 'numeric' });
  }

  previousMonth() {
    this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() - 1, 1);
    this.loadAppointments();
  }

  nextMonth() {
    this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 1);
    this.loadAppointments();
  }

  loadAppointments() {
    if (!this.doctorId) return;
    this.loading = true;
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth() + 1;
    this.http.get<any[]>(`${environment.apiUrl}/appointments/doctor/${this.doctorId}/year/${year}/month/${month}`)
      .subscribe({
        next: (data) => {
          this.appointments = data;
          this.generateCalendar();
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
  }

  generateCalendar() {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();
    const firstDay = new Date(year, month, 1).getDay(); // 0(Sun) - 6(Sat)
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    let currentWeek: any[] = [];
    this.calendarWeeks = [];

    // Pad first week
    for (let i = 0; i < firstDay; i++) {
      currentWeek.push(null);
    }

    // Fill days
    for (let day = 1; day <= daysInMonth; day++) {
      const dateObj = new Date(year, month, day);
      const dayApts = this.appointments.filter(a => {
        if (!a.date) return false;
        const datePart = a.date.split('T')[0];
        const parts = datePart.split('-');
        if (parts.length === 3) {
          return parseInt(parts[0], 10) === year &&
            parseInt(parts[1], 10) === (month + 1) &&
            parseInt(parts[2], 10) === day;
        }
        return false;
      });

      currentWeek.push({
        day: day,
        date: dateObj,
        appointments: dayApts
      });

      if (currentWeek.length === 7) {
        this.calendarWeeks.push(currentWeek);
        currentWeek = [];
      }
    }

    // Pad last week
    if (currentWeek.length > 0) {
      while (currentWeek.length < 7) {
        currentWeek.push(null);
      }
      this.calendarWeeks.push(currentWeek);
    }
  }

  formatTime(time: string) {
    if (!time) return '';
    const parts = time.split(':');
    return `${parts[0]}:${parts[1]}`;
  }

  getStatusClass(status: number) {
    switch (status) {
      case 0: return 'status-0'; // Agendado
      case 1: return 'status-1'; // Em Espera
      case 2: return 'status-2'; // Atendido
      case 3: return 'status-3'; // Faltou
      case 4: return 'status-4'; // Cancelado
      default: return '';
    }
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

  onPatientCreated() {
    // Optionally show a success toast
  }

  onAppointmentCreated() {
    this.loadAppointments();
  }

  logout() {
    localStorage.removeItem('doctorId');
    this.router.navigate(['/login']);
  }
}
