import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  currentDate = new Date();
  doctorId: number | null = null;
  loading = false;

  public lineChartData: any = {
    datasets: [],
    labels: []
  };

  public lineChartOptions: any = {
    responsive: true,
    maintainAspectRatio: false
  };

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    const id = localStorage.getItem('doctorId');
    if (!id) {
      this.router.navigate(['/login']);
      return;
    }
    this.doctorId = parseInt(id, 10);
    this.loadData();
  }

  get monthName(): string {
    return this.currentDate.toLocaleString('pt-BR', { month: 'long', year: 'numeric' });
  }

  previousMonth() {
    this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() - 1, 1);
    this.loadData();
  }

  nextMonth() {
    this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 1);
    this.loadData();
  }

  loadData() {
    if (!this.doctorId) return;
    this.loading = true;
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth() + 1;
    this.http.get<any[]>(`http://localhost:5111/api/appointments/doctor/${this.doctorId}/year/${year}/month/${month}`)
      .subscribe({
        next: (data) => {
          this.processChartData(data, year, month - 1);
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
  }

  processChartData(appointments: any[], year: number, month: number) {
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const labels = Array.from({ length: daysInMonth }, (_, i) => (i + 1).toString());

    const marcadosData = new Array(daysInMonth).fill(0);
    const realizadosData = new Array(daysInMonth).fill(0);
    const naoAtendidosData = new Array(daysInMonth).fill(0);

    appointments.forEach(a => {
      if (!a.date) return;
      const datePart = a.date.split('T')[0];
      const parts = datePart.split('-');
      if (parts.length === 3) {
        const d = parseInt(parts[2], 10);
        const dayIndex = d - 1;

        if (a.status === 0 || a.status === 1) {
          marcadosData[dayIndex]++;
        } else if (a.status === 2) {
          realizadosData[dayIndex]++;
        } else if (a.status === 3 || a.status === 4) {
          naoAtendidosData[dayIndex]++;
        }
      }
    });

    this.lineChartData = {
      labels: labels,
      datasets: [
        {
          data: marcadosData,
          label: 'Marcados',
          borderColor: '#17a2b8',
          backgroundColor: 'rgba(23, 162, 184, 0.2)',
          fill: false,
          tension: 0.3
        },
        {
          data: realizadosData,
          label: 'Realizados',
          borderColor: '#28a745',
          backgroundColor: 'rgba(40, 167, 69, 0.2)',
          fill: false,
          tension: 0.3
        },
        {
          data: naoAtendidosData,
          label: 'Não Atendidos',
          borderColor: '#dc3545',
          backgroundColor: 'rgba(220, 53, 69, 0.2)',
          fill: false,
          tension: 0.3
        }
      ]
    };
  }

  goBack() {
    this.router.navigate(['/agenda']);
  }
}
