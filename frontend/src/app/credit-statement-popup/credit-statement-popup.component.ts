import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CreditService, CreditTransaction } from '../credit.service';

@Component({
  selector: 'app-credit-statement-popup',
  templateUrl: './credit-statement-popup.component.html',
  styleUrls: ['./credit-statement-popup.component.css']
})
export class CreditStatementPopupComponent implements OnInit {
  @Input() doctorId!: number;
  @Output() close = new EventEmitter<void>();

  transactions: CreditTransaction[] = [];
  loading = true;

  constructor(private creditService: CreditService) {}

  ngOnInit(): void {
    if (this.doctorId) {
      this.loadStatement();
    }
  }

  loadStatement() {
    this.loading = true;
    this.creditService.getStatement(this.doctorId).subscribe({
      next: (data) => {
        this.transactions = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  formatTime(time: string | undefined): string {
    if (!time) return '';
    const parts = time.split(':');
    return `${parts[0]}:${parts[1]}`;
  }

  formatDate(date: string | undefined): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR');
  }

  onClose() {
    this.close.emit();
  }
}
