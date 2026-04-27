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
  isDeleted: boolean;
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

  // Credit plan update popup
  showCreditPopup = false;
  selectedDoctor: DoctorReportItem | null = null;
  newPlanCredits: number | null = null;
  confirmText = '';
  saving = false;
  successMessage = '';
  popupError = '';

  // Status toggle
  viewMode: 'active' | 'inactive' = 'active';
  showStatusPopup = false;
  statusAction: 'activate' | 'delete' = 'delete';
  statusConfirmText = '';
  statusSaving = false;
  statusError = '';

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
          this.filterDoctors();
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

  setViewMode(mode: 'active' | 'inactive') {
    this.viewMode = mode;
    this.filterDoctors();
  }

  filterDoctors() {
    let filtered = this.doctors;
    
    if (this.viewMode === 'active') {
      filtered = filtered.filter(d => !d.isDeleted);
    } else {
      filtered = filtered.filter(d => d.isDeleted);
    }

    const term = this.searchTerm.toLowerCase().trim();
    if (term) {
      filtered = filtered.filter(
        (d) =>
          d.name.toLowerCase().includes(term) ||
          d.email.toLowerCase().includes(term)
      );
    }
    
    this.filteredDoctors = filtered;
  }

  onSearch() {
    this.filterDoctors();
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

  // ===== Credit Plan Popup Methods =====

  openCreditPopup(doctor: DoctorReportItem) {
    this.selectedDoctor = doctor;
    this.newPlanCredits = doctor.planCredits;
    this.confirmText = '';
    this.saving = false;
    this.successMessage = '';
    this.popupError = '';
    this.showCreditPopup = true;
  }

  closeCreditPopup() {
    this.showCreditPopup = false;
    this.selectedDoctor = null;
    this.confirmText = '';
    this.popupError = '';
    this.successMessage = '';
  }

  get isConfirmValid(): boolean {
    return this.confirmText.trim().toLowerCase() === 'autorizo';
  }

  submitCreditUpdate() {
    if (!this.selectedDoctor || !this.isConfirmValid || this.newPlanCredits === null) return;

    if (this.newPlanCredits < 0) {
      this.popupError = 'O valor não pode ser negativo.';
      return;
    }

    const requestingDoctorId = localStorage.getItem('doctorId');
    if (!requestingDoctorId) return;

    this.saving = true;
    this.popupError = '';
    this.successMessage = '';

    this.http
      .patch<any>(
        `${environment.apiUrl}/admin/doctor/${this.selectedDoctor.id}/update-plan?requestingDoctorId=${requestingDoctorId}`,
        { newPlanCredits: this.newPlanCredits }
      )
      .subscribe({
        next: (res) => {
          this.saving = false;
          this.successMessage = `Plano de ${this.selectedDoctor!.name} atualizado para ${res.planCredits} créditos!`;
          // Update local data
          const doc = this.doctors.find(d => d.id === this.selectedDoctor!.id);
          if (doc) {
            doc.planCredits = res.planCredits;
            doc.credits = res.credits;
          }
          this.calculateTotals();
          // Auto close after 2s
          setTimeout(() => this.closeCreditPopup(), 2000);
        },
        error: (err) => {
          this.saving = false;
          this.popupError = err.error?.message || err.error || 'Erro ao atualizar plano.';
        }
      });
  }

  // ===== Status Popup Methods =====

  openStatusPopup(doctor: DoctorReportItem, action: 'activate' | 'delete') {
    this.selectedDoctor = doctor;
    this.statusAction = action;
    this.showStatusPopup = true;
    this.statusConfirmText = '';
    this.statusSaving = false;
    this.statusError = '';
    this.successMessage = '';
  }

  closeStatusPopup() {
    this.showStatusPopup = false;
    this.selectedDoctor = null;
    this.statusConfirmText = '';
    this.statusError = '';
  }

  get isStatusConfirmValid(): boolean {
    const term = this.statusConfirmText.trim().toLowerCase();
    if (this.statusAction === 'delete') {
      return term === 'excluir';
    } else {
      return term === 'ativar';
    }
  }

  submitStatusUpdate() {
    if (!this.selectedDoctor || !this.isStatusConfirmValid) return;

    const requestingDoctorId = localStorage.getItem('doctorId');
    if (!requestingDoctorId) return;

    this.statusSaving = true;
    this.statusError = '';
    this.successMessage = '';

    const payload = { isDeleted: this.statusAction === 'delete' };

    this.http
      .patch<any>(
        `${environment.apiUrl}/admin/doctor/${this.selectedDoctor.id}/toggle-status?requestingDoctorId=${requestingDoctorId}`,
        payload
      )
      .subscribe({
        next: (res) => {
          this.statusSaving = false;
          this.successMessage = res.message || 'Status atualizado com sucesso!';
          
          const doc = this.doctors.find(d => d.id === this.selectedDoctor!.id);
          if (doc) {
            doc.isDeleted = res.isDeleted;
          }
          
          this.filterDoctors();
          this.calculateTotals();
          
          setTimeout(() => {
            this.closeStatusPopup();
            this.successMessage = '';
          }, 2000);
        },
        error: (err) => {
          this.statusSaving = false;
          this.statusError = err.error?.message || err.error || 'Erro ao alterar status do médico.';
        }
      });
  }
}
