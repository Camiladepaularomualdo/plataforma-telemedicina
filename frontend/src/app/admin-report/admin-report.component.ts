import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { AuthService } from '../auth.service';

export interface DoctorReportItem {
  id: number;
  name: string;
  email: string;
  credits: number;
  planCredits: number;
  createdAt: string;
  rule: string;
  totalCreditsUsed: number;
  totalPatients: number;
  totalAppointments: number;
}

@Component({
  selector: 'app-admin-report',
  templateUrl: './admin-report.component.html',
  styleUrls: ['./admin-report.component.css']
})
export class AdminReportComponent implements OnInit {
  doctors: DoctorReportItem[] = [];
  filteredDoctors: DoctorReportItem[] = [];
  loading = true;
  errorMessage = '';
  searchTerm = '';

  // Summary totals
  totalDoctors = 0;
  totalCreditsUsedAll = 0;
  totalAppointmentsAll = 0;
  totalPatientsAll = 0;

  constructor(
    private http: HttpClient,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    if (!this.authService.canAccessAdmin()) {
      this.router.navigate(['/agenda']);
      return;
    }

    this.loadReport();
  }

  loadReport() {
    const doctorId = localStorage.getItem('doctorId');
    if (!doctorId) {
      this.router.navigate(['/login']);
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.http
      .get<DoctorReportItem[]>(
        `${environment.apiUrl}/admin/doctors-report?requestingDoctorId=${doctorId}`
      )
      .subscribe({
        next: (data) => {
          this.doctors = data;
          this.filteredDoctors = data;
          this.calculateTotals();
          this.loading = false;
        },
        error: (err) => {
          this.errorMessage =
            err.error || 'Erro ao carregar relatório. Verifique suas permissões.';
          this.loading = false;
        }
      });
  }

  calculateTotals() {
    this.totalDoctors = this.doctors.length;
    this.totalCreditsUsedAll = this.doctors.reduce(
      (sum, d) => sum + d.totalCreditsUsed,
      0
    );
    this.totalAppointmentsAll = this.doctors.reduce(
      (sum, d) => sum + d.totalAppointments,
      0
    );
    this.totalPatientsAll = this.doctors.reduce(
      (sum, d) => sum + d.totalPatients,
      0
    );
  }

  onSearch() {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredDoctors = this.doctors;
    } else {
      this.filteredDoctors = this.doctors.filter(
        (d) =>
          d.name.toLowerCase().includes(term) ||
          d.email.toLowerCase().includes(term)
      );
    }
  }

  formatDate(date: string): string {
    if (!date) return '—';
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR');
  }

  getRoleBadgeClass(rule: string): string {
    switch (rule) {
      case 'adm':
        return 'role-adm';
      case 'all':
        return 'role-all';
      default:
        return 'role-usr';
    }
  }

  getRoleLabel(rule: string): string {
    switch (rule) {
      case 'adm':
        return 'Admin';
      case 'all':
        return 'Completo';
      default:
        return 'Usuário';
    }
  }

  goBack() {
    this.router.navigate(['/agenda']);
  }
}
