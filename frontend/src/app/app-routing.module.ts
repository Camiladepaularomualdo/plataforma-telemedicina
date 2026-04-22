import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { AgendaComponent } from './agenda/agenda.component';
import { AppointmentsListComponent } from './appointments-list/appointments-list.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HomeComponent } from './home/home.component';
import { FaqComponent } from './faq/faq.component';
import { AdminReportComponent } from './admin-report/admin-report.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'agenda', component: AgendaComponent },
  { path: 'appointments', component: AppointmentsListComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'faq', component: FaqComponent },
  { path: 'admin-report', component: AdminReportComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
