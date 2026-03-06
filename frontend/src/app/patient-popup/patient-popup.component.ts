import { Component, Input, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-patient-popup',
  templateUrl: './patient-popup.component.html',
  styleUrls: ['./patient-popup.component.css']
})
export class PatientPopupComponent {
  @Input() appointment: any;
  @Output() close = new EventEmitter<void>();
  @Output() statusChanged = new EventEmitter<void>();

  constructor(private http: HttpClient) { }

  closePopup() {
    this.close.emit();
  }

  onStatusChange(newStatus: any) {
    const statusNum = parseInt(newStatus, 10);
    this.http.patch(`http://localhost:5111/api/appointments/${this.appointment.id}/status/${statusNum}`, {}).subscribe({
      next: () => {
        this.appointment.status = statusNum;
        this.statusChanged.emit();
      },
      error: (err) => {
        console.error('Failed to update status', err);
      }
    });
  }

  // To display status as a friendly string
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
}
