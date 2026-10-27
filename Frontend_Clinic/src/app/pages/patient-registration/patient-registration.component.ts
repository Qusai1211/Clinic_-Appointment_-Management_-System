import { Component, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { NotificationService } from '../../services/notification.service';
import { Patient } from '../../models/clinic.models';

@Component({
  selector: 'app-patient-registration',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './patient-registration.component.html',
  styleUrls: ['./patient-registration.component.less']
})
export class PatientRegistrationComponent {
  private api = inject(ApiService);
  private notify = inject(NotificationService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  fullName = '';
  phoneNumber = '';
  email = '';
  dateOfBirth = '';

  isSubmitting = false;
  registeredPatient: Patient | null = null;
  errorMessage = '';

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.fullName.trim()) {
      this.errorMessage = 'Please enter patient full name.';
      return;
    }
    if (!this.phoneNumber.trim()) {
      this.errorMessage = 'Please enter patient phone number.';
      return;
    }
    if (!this.email.trim() || !this.email.includes('@')) {
      this.errorMessage = 'Please enter a valid email address.';
      return;
    }
    if (!this.dateOfBirth) {
      this.errorMessage = 'Please select date of birth.';
      return;
    }

    this.isSubmitting = true;

    this.api.registerPatient({
      fullName: this.fullName.trim(),
      phoneNumber: this.phoneNumber.trim(),
      email: this.email.trim(),
      dateOfBirth: this.dateOfBirth
    }).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        if (res.success && res.patient) {
          this.registeredPatient = res.patient;
          this.notify.success('Patient registered successfully!');
        } else {
          this.errorMessage = res.message || 'Registration failed.';
        }
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err.error?.message || 'Server error occurred during patient registration.';
        this.notify.error(this.errorMessage);
        this.cdr.markForCheck();
      }
    });
  }

  resetForm(): void {
    this.fullName = '';
    this.phoneNumber = '';
    this.email = '';
    this.dateOfBirth = '';
    this.registeredPatient = null;
    this.errorMessage = '';
    this.cdr.markForCheck();
  }
}
