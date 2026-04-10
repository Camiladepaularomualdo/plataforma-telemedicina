import { Component, Input, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

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
    this.http.patch(`${environment.apiUrl}/appointments/${this.appointment.id}/status/${statusNum}`, {}).subscribe({
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

  iniciarAtendimento() {
    //alert(`${environment.apiUrl}/appointments/${this.appointment.id}/generate-meeting`);
    this.http.post(`${environment.apiUrl}/appointments/${this.appointment.id}/generate-meeting`, {}).subscribe({
      next: (res: any) => {
        this.appointment.meetingUrl = res.meetingUrl;
        this.statusChanged.emit();
      },
      error: (err) => {
        console.error('Failed to generate meeting URL', err);
        alert('Erro ao gerar URL da consulta. Verifique se a API Key do Whereby está configurada.');
      }
    });
  }

  copyUrl() {
    if (this.appointment.meetingUrl) {
      navigator.clipboard.writeText(this.appointment.meetingUrl).then(() => {
        alert('URL copiada para a área de transferência!');
      });
    }
  }

  showEmailConfig = false;
  isSendingEmail = false;

  enviarPorEmail() {
    this.isSendingEmail = true;

    // First, check if the doctor has Gmail configured
    this.http.get<{ hasConfig: boolean, gmailAddress: string }>(`${environment.apiUrl}/doctors/${this.appointment.doctorId}/gmail-config`).subscribe({
      next: (config) => {
        if (config.hasConfig) {
          // Doctor has config, send the email
          this.executarEnvio();
        } else {
          // Doctor doesn't have config, show the setup modal
          this.isSendingEmail = false;
          this.showEmailConfig = true;
        }
      },
      error: (err) => {
        this.isSendingEmail = false;
        console.error('Failed to check config', err);
        alert('Erro ao verificar configurações de email.');
      }
    });
  }

  executarEnvio() {
    this.http.post(`${environment.apiUrl}/appointments/${this.appointment.id}/send-email`, {}).subscribe({
      next: () => {
        this.isSendingEmail = false;
        alert('Email enviado com sucesso para o paciente!');
      },
      error: (err) => {
        this.isSendingEmail = false;
        console.error('Failed to send email', err);
        alert('Erro ao enviar email. ' + (err.error || 'Verifique o console para mais detalhes.'));
      }
    });
  }

  onConfigSaved() {
    this.showEmailConfig = false;
    // Retry sending the email now that config is saved
    this.enviarPorEmail();
  }
}
