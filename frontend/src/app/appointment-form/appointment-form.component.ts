import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-appointment-form',
  templateUrl: './appointment-form.component.html',
  styleUrls: ['./appointment-form.component.css']
})
export class AppointmentFormComponent implements OnInit {
  @Input() doctorId!: number;
  @Output() close = new EventEmitter<void>();
  @Output() created = new EventEmitter<any>();

  patients: any[] = [];
  appointment = { patientId: null, date: '', time: '' };
  errorStr = '';

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.loadPatients();
  }

  loadPatients() {
    this.http.get<any[]>('http://localhost:5111/api/patients').subscribe(data => {
      this.patients = data;
    });
  }

  save() {
    if (!this.appointment.patientId || !this.appointment.date || !this.appointment.time) {
      this.errorStr = 'Preencha todos os campos';
      return;
    }

    const payload = {
      doctorId: this.doctorId,
      patientId: parseInt(this.appointment.patientId, 10),
      date: this.appointment.date,
      time: this.appointment.time + ':00' // SQL Server TimeSpan expects hh:mm:ss format
    };

    this.http.post<any>('http://localhost:5111/api/appointments', payload).subscribe({
      next: (res) => {
        this.created.emit(res);
        this.close.emit();
      },
      error: (err) => {
        this.errorStr = err.error || 'Erro ao agendar';
      }
    });
  }
}
