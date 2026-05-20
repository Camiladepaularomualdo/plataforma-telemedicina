import { Component, Output, EventEmitter, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-patient-form',
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.css']
})
export class PatientFormComponent {
  @Input() doctorId!: number;
  @Output() close = new EventEmitter<void>();
  @Output() created = new EventEmitter<any>();

  patient = { name: '', email: '', phone: '', birthDate: '', cpf: '' };
  errorStr = '';

  constructor(private http: HttpClient) { }

  save() {
    const payload = { ...this.patient, doctorId: this.doctorId };
    this.http.post<any>(`${environment.apiUrl}/patients`, payload).subscribe({
      next: (res) => {
        this.created.emit(res);
        this.close.emit();
      },
      error: (err) => {
        let errMsg = 'Erro ao cadastrar paciente';
        if (typeof err.error === 'string') {
          errMsg = err.error;
        } else if (err.error && err.error.errors) {
          errMsg = Object.values(err.error.errors).map((e: any) => e.join(', ')).join(' | ');
        } else if (err.error && err.error.message) {
          errMsg = err.error.message;
        }
        this.errorStr = errMsg;
      }
    });
  }
}
