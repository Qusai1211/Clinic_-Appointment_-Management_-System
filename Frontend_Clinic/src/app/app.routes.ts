import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  // Public Clinical Pages
  {
    path: '',
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent),
    title: 'Al-Amal Clinic | Home'
  },
  {
    path: 'home',
    redirectTo: '',
    pathMatch: 'full'
  },
  {
    path: 'doctors',
    loadComponent: () => import('./pages/doctors/doctors.component').then(m => m.DoctorsComponent),
    title: 'Find Doctors | Al-Amal Clinic'
  },
  {
    path: 'doctors/:id',
    loadComponent: () => import('./pages/doctor-details/doctor-details.component').then(m => m.DoctorDetailsComponent),
    title: 'Doctor Details & Schedule | Al-Amal Clinic'
  },
  {
    path: 'register-patient',
    loadComponent: () => import('./pages/patient-registration/patient-registration.component').then(m => m.PatientRegistrationComponent),
    title: 'Patient Registration | Al-Amal Clinic'
  },
  {
    path: 'book',
    loadComponent: () => import('./pages/book-appointment/book-appointment.component').then(m => m.BookAppointmentComponent),
    title: 'Book Appointment | Al-Amal Clinic'
  },
  {
    path: 'confirmation/:referenceNumber',
    loadComponent: () => import('./pages/appointment-confirmation/appointment-confirmation.component').then(m => m.AppointmentConfirmationComponent),
    title: 'Appointment Confirmation | Al-Amal Clinic'
  },
  {
    path: 'appointments',
    loadComponent: () => import('./pages/appointments/appointments.component').then(m => m.AppointmentsComponent),
    title: 'Clinic Appointments | Al-Amal Clinic'
  },

  // Administrative Portal
  {
    path: 'admin/login',
    loadComponent: () => import('./pages/admin/admin-login/admin-login.component').then(m => m.AdminLoginComponent),
    title: 'Admin Login | Al-Amal Clinic'
  },
  {
    path: 'admin/dashboard',
    loadComponent: () => import('./pages/admin/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
    canActivate: [authGuard],
    title: 'Admin Dashboard | Al-Amal Clinic'
  },
  {
    path: 'admin/doctors',
    loadComponent: () => import('./pages/admin/manage-doctors/manage-doctors.component').then(m => m.ManageDoctorsComponent),
    canActivate: [authGuard],
    title: 'Manage Doctors | Admin Portal'
  },
  {
    path: 'admin/specialties',
    loadComponent: () => import('./pages/admin/manage-specialties/manage-specialties.component').then(m => m.ManageSpecialtiesComponent),
    canActivate: [authGuard],
    title: 'Manage Specialties | Admin Portal'
  },
  {
    path: 'admin/patients',
    loadComponent: () => import('./pages/admin/manage-patients/manage-patients.component').then(m => m.ManagePatientsComponent),
    canActivate: [authGuard],
    title: 'Manage Patients | Admin Portal'
  },
  {
    path: 'admin/appointments',
    loadComponent: () => import('./pages/admin/manage-appointments/manage-appointments.component').then(m => m.ManageAppointmentsComponent),
    canActivate: [authGuard],
    title: 'Manage Appointments | Admin Portal'
  },

  // Not Found Route
  {
    path: '404',
    loadComponent: () => import('./pages/not-found/not-found.component').then(m => m.NotFoundComponent),
    title: 'Page Not Found | Al-Amal Clinic'
  },
  {
    path: '**',
    loadComponent: () => import('./pages/not-found/not-found.component').then(m => m.NotFoundComponent),
    title: 'Page Not Found | Al-Amal Clinic'
  }
];
