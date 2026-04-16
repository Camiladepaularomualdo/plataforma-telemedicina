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

@Injectable({
  providedIn: 'root'
})
export class CreditService {
  private apiUrl = `${environment.apiUrl}/credits`;
  private creditsSubject = new BehaviorSubject<number | null>(null);

  credits$ = this.creditsSubject.asObservable();

  constructor(private http: HttpClient) {}

  updateCreditsForDoctor(doctorId: number) {
    this.http.get<{credits: number}>(`${this.apiUrl}/doctor/${doctorId}/balance`).subscribe(res => {
      this.creditsSubject.next(res.credits);
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
