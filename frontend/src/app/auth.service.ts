import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  /**
   * Returns the current doctor's role from localStorage.
   * Defaults to 'usr' if not set.
   */
  getRule(): string {
    return localStorage.getItem('doctorRule') || 'usr';
  }

  /**
   * Admin role: access to Dashboard/metrics, but NOT to
   * appointments, patient registration, or scheduling.
   */
  isAdmin(): boolean {
    const rule = this.getRule();
    return rule === 'adm';
  }

  /**
   * Standard user role: access to appointments, patients,
   * and scheduling, but NOT to Dashboard.
   */
  isUser(): boolean {
    return this.getRule() === 'usr';
  }

  /**
   * Full access role: access to everything (admin + user features).
   */
  isAll(): boolean {
    return this.getRule() === 'all';
  }

  /**
   * Can access user-facing features: appointments, patient registration,
   * scheduling, credit statement.
   * Allowed for: usr, all
   */
  canAccessAttendance(): boolean {
    const rule = this.getRule();
    return rule === 'usr' || rule === 'all';
  }

  /**
   * Can access admin features: Dashboard/metrics.
   * Allowed for: adm, all
   */
  canAccessAdmin(): boolean {
    const rule = this.getRule();
    return rule === 'adm' || rule === 'all';
  }

  /**
   * Generic permission check against a list of allowed roles.
   */
  hasPermission(allowedRoles: string[]): boolean {
    return allowedRoles.includes(this.getRule());
  }

  /**
   * Clears all auth-related data from localStorage.
   */
  clearAuth(): void {
    localStorage.removeItem('doctorId');
    localStorage.removeItem('doctorRule');
  }
}
