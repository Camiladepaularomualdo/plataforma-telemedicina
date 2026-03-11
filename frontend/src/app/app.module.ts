import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { AgendaComponent } from './agenda/agenda.component';
import { PatientPopupComponent } from './patient-popup/patient-popup.component';
import { PatientFormComponent } from './patient-form/patient-form.component';
import { AppointmentFormComponent } from './appointment-form/appointment-form.component';
import { AppointmentsListComponent } from './appointments-list/appointments-list.component';
import { EmailConfigPopupComponent } from './email-config-popup/email-config-popup.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { NgChartsModule } from 'ng2-charts';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    AgendaComponent,
    PatientPopupComponent,
    PatientFormComponent,
    AppointmentFormComponent,
    AppointmentsListComponent,
    EmailConfigPopupComponent,
    DashboardComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    NgChartsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
