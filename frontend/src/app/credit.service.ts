import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface CreditTransaction {
  id: number;
  amount: number;
  description: string;
  createdAt: string;
  patientName: string;
  appointmentDate?: string;
  appointmentTime?: string;
}

export interface BalanceDetails {
  credits: number;
  planCredits: number;
  lastRenewalDate?: string;
  nextRenewalDate?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CreditService {
  private apiUrl = `${environment.apiUrl}/credits`;
  private creditsSubject = new BehaviorSubject<number | null>(null);
  private balanceDetailsSubject = new BehaviorSubject<BalanceDetails | null>(null);

  credits$ = this.creditsSubject.asObservable();
  balanceDetails$ = this.balanceDetailsSubject.asObservable();

  constructor(private http: HttpClient) {}

  updateCreditsForDoctor(doctorId: number) {
    this.http.get<BalanceDetails>(`${this.apiUrl}/doctor/${doctorId}/balance`).subscribe(res => {
      this.creditsSubject.next(res.credits);
      this.balanceDetailsSubject.next(res);
    });
  }

  getStatement(doctorId: number): Observable<CreditTransaction[]> {
    return this.http.get<CreditTransaction[]>(`${this.apiUrl}/doctor/${doctorId}/statement`);
  }

  decrementLocalCredit() {
    const current = this.creditsSubject.value;
    if (current !== null && current > 0) {
      this.creditsSubject.next(current - 1);
    }
  }
}
