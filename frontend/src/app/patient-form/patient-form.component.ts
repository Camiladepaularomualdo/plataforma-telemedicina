import { Component, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-patient-form',
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.css']
})
export class PatientFormComponent {
  @Output() close = new EventEmitter<void>();
  @Output() created = new EventEmitter<any>();

  patient = { name: '', email: '', phone: '', birthDate: '', cpf: '' };
  errorStr = '';

  constructor(private http: HttpClient) { }

  save() {
    this.http.post<any>(`${environment.apiUrl}/patients`, this.patient).subscribe({
      next: (res) => {
        this.created.emit(res);
        this.close.emit();
      },
      error: (err) => {
        this.errorStr = err.error || 'Erro ao cadastrar paciente';
      }
    });
  }
}
