import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './components/auth/login/login.component';
import { AppComponent } from './app.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { AcceuilComponent } from './acceuil/acceuil.component';
import { AdminLoginComponent } from './admin-login/admin-login.component';
import { AdminRegisterComponent } from './admin-register/admin-register.component';
import { AdminComponent } from './admin/admin.component';
import { UserFoAdminComponent } from './users/userFoAdmin.component';
import { ProgramsComponent } from './programs/programs.component';
import { AddProgramComponent } from './add-program/add-program.component';
import { adminGuard } from './guards/admin.guard';
import {ProgramdetailscardComponent} from './programdetailscard/programdetailscard.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { UsersDataComponent } from './users-data/users-data.component';
import { ApplicationsComponent } from './applications/applications.component';
import { MyApplicationsComponent } from './my-applications/my-applications.component';
import { AddApplicationComponent } from './add-application/add-application.component';
import { UniversityApplicationComponent } from './services-detail/university-application/university-application.component';
import { VisaProcedureComponent } from './services-detail/visa-procedure/visa-procedure.component';
import { HousingSearchComponent } from './services-detail/housing-search/housing-search.component';
import { TravelInsuranceComponent } from './services-detail/travel-insurance/travel-insurance.component';
import { ScholarshipGuidanceComponent } from './services-detail/scholarship-guidance/scholarship-guidance.component';
import { InterviewPreparationComponent } from './services-detail/interview-preparation/interview-preparation.component';
import { OfficialTranslationComponent } from './services-detail/official-translation/official-translation.component';
import { HotelTicketReservationComponent } from './services-detail/hotel-ticket-reservation/hotel-ticket-reservation.component';
export const routes: Routes = [
    {path:'programs', component: ProgramsComponent, canActivate: [adminGuard]},
    {path:'add-program', component: AddProgramComponent, canActivate: [adminGuard]},
    {path:'add-application/:id', component: AddApplicationComponent, canActivate: [authGuard]},
    {path:'AdminDashboard', component: AdminComponent, canActivate: [adminGuard]},
    {path:'users', component: UserFoAdminComponent, canActivate: [adminGuard]},
    {path: 'AdminLogin', component: AdminLoginComponent},
    {path: 'AdminJdid', component: AdminRegisterComponent},
    {path: 'login', component: LoginComponent },
    {path: 'registerservice', component: RegisterComponent},
    {path : 'Acceuil', component: AcceuilComponent},
    {path : 'programdetails/:id', component: ProgramdetailscardComponent},
    {path: 'user-profile', component: UserProfileComponent},
    {path: 'UsersData', component: UsersDataComponent},
    {path:'applications',component: ApplicationsComponent},
    {path:'my-applications',component: MyApplicationsComponent, canActivate: [authGuard]},
    {path: 'service/university-application', component: UniversityApplicationComponent},
    {path: 'service/visa-procedure', component: VisaProcedureComponent},
    {path: 'service/housing-search', component: HousingSearchComponent},
    {path: 'service/travel-insurance', component: TravelInsuranceComponent},
    {path: 'service/scholarship-guidance', component: ScholarshipGuidanceComponent},
    {path: 'service/interview-preparation', component: InterviewPreparationComponent},
    {path: 'service/official-translation', component: OfficialTranslationComponent},
    {path: 'service/hotel-ticket-reservation', component: HotelTicketReservationComponent},
    {path: '**', component: LoginComponent, pathMatch: 'full' },
];
