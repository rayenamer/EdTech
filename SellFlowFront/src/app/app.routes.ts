import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './components/auth/login/login.component';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { AcceuilComponent } from './acceuil/acceuil.component';
import { AdminLoginComponent } from './admin-login/admin-login.component';
import { AdminRegisterComponent } from './admin-register/admin-register.component';
import { AdminComponent } from './admin/admin.component';
import { OurservicesComponent } from './ourservices/ourservices.component';
import { AboutusComponent } from './aboutus/aboutus.component';
import { CareersComponent } from './careers/careers.component';
import { CommunityComponent } from './community/community.component';
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
export const routes: Routes = [
    {path: '', component: HomeComponent },
    {path:'programs', component: ProgramsComponent, canActivate: [adminGuard]},
    {path:'add-program', component: AddProgramComponent, canActivate: [adminGuard]},
    {path:'add-application/:id', component: AddApplicationComponent, canActivate: [authGuard]},
    {path:'community',component:CommunityComponent},
    {path: 'careers', component: CareersComponent },
    {path: 'aboutus', component: AboutusComponent },
    {path:'services', component:OurservicesComponent},
    {path:'AdminDashboard', component: AdminComponent, canActivate: [adminGuard]},
    {path:'users', component: UserFoAdminComponent, canActivate: [adminGuard]},
    {path: 'AdminLogin', component: AdminLoginComponent},
    {path: 'AdminJdid', component: AdminRegisterComponent},
    {path: 'login', component: LoginComponent },
    {path: 'registerservice', component: RegisterComponent},
    {path: 'resetpass', component: ResetPasswordComponent },
    {path: 'ForgotPass', component: ForgotPasswordComponent},
    {path : 'Acceuil', component: AcceuilComponent},
    {path : 'programdetails/:id', component: ProgramdetailscardComponent},
    {path: 'user-profile', component: UserProfileComponent},
    {path: 'UsersData', component: UsersDataComponent},
    {path:'applications',component: ApplicationsComponent},
    {path:'my-applications',component: MyApplicationsComponent, canActivate: [authGuard]},
    {path: '**', component: HomeComponent, pathMatch: 'full' },
];
