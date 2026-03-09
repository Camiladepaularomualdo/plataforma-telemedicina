import { Component, Output, EventEmitter, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-email-config-popup',
  templateUrl: './email-config-popup.component.html',
  styleUrls: ['./email-config-popup.component.css']
})
export class EmailConfigPopupComponent {
  @Input() doctorId!: number;
  @Output() configSaved = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  gmailAddress = '';
  gmailAppPassword = '';
  isSaving = false;
  errorMsg = '';

  constructor(private http: HttpClient) { }

  saveConfig() {
    if (!this.gmailAddress || !this.gmailAppPassword) {
      this.errorMsg = 'Preencha todos os campos.';
      return;
    }

    this.isSaving = true;
    this.errorMsg = '';

    const payload = {
      gmailAddress: this.gmailAddress,
      gmailAppPassword: this.gmailAppPassword
    };

    this.http.post(`http://localhost:5111/api/doctors/${this.doctorId}/gmail-config`, payload).subscribe({
      next: () => {
        this.isSaving = false;
        this.configSaved.emit();
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMsg = 'Erro ao salvar configurações. Tente novamente.';
        console.error(err);
      }
    });
  }

  closePopup() {
    this.close.emit();
  }
}
