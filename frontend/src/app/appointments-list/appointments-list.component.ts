import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-appointments-list',
  templateUrl: './appointments-list.component.html',
  styleUrls: ['./appointments-list.component.css']
})
export class AppointmentsListComponent implements OnInit {
  appointments: any[] = [];
  filteredAppointments: any[] = [];
  doctorId: number | null = null;
  loading = false;

  // Filters
  filterStatus: string = '';
  filterDay: string = '';
  filterMonth: string = '';
  filterName: string = '';

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

  loadAppointments() {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/appointments/doctor/${this.doctorId}/all`)
      .subscribe({
        next: (data) => {
          this.appointments = data;
          this.applyFilters();
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
  }

  applyFilters() {
    this.filteredAppointments = this.appointments.filter(a => {
      let match = true;

      // Status
      if (this.filterStatus !== '') {
        if (a.status.toString() !== this.filterStatus) match = false;
      }

      // Day and Month extraction from Date string
      if (a.date && (this.filterDay || this.filterMonth)) {
        const d = new Date(a.date);

        if (this.filterDay) {
          const dayNum = parseInt(this.filterDay, 10);
          if (d.getDate() !== dayNum) match = false;
        }

        if (this.filterMonth) {
          const monthNum = parseInt(this.filterMonth, 10); // 1 to 12
          if ((d.getMonth() + 1) !== monthNum) match = false;
        }
      }

      // Name
      if (this.filterName && this.filterName.trim() !== '') {
        const searchStr = this.filterName.toLowerCase().trim();
        const name = a.patient?.name?.toLowerCase() || '';
        if (!name.includes(searchStr)) match = false;
      }

      return match;
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

  goBack() {
    this.router.navigate(['/agenda']);
  }

  copyUrl(url: string) {
    if (url) {
      navigator.clipboard.writeText(url).then(() => {
        alert('URL copiada para a área de transferência!');
      });
    }
  }
}
