import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { NotificationService } from '../../services/notification.service';
import { Doctor, AppointmentSlot, Patient } from '../../models/clinic.models';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './book-appointment.component.html',
  styleUrls: ['./book-appointment.component.less']
})
export class BookAppointmentComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  doctors: Doctor[] = [];
  patients: Patient[] = [];
  availableSlots: AppointmentSlot[] = [];

  selectedDoctorId: number = 0;
  selectedDoctor: Doctor | null = null;
  selectedSlotId: number = 0;
  selectedSlot: AppointmentSlot | null = null;
  selectedDate: string = '';

  patientMode: 'new' | 'existing' = 'new';
  selectedPatientId: number = 0;

  patientName = '';
  patientPhone = '';
  patientEmail = '';
  patientDateOfBirth = '';

  isLoading = true;
  isSubmitting = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.isLoading = true;

    this.api.getDoctors().subscribe({
      next: (docs) => {
        this.doctors = docs;
        this.api.getPatients().subscribe({
          next: (pats) => {
            this.patients = pats;
            this.handleQueryParams();
            this.isLoading = false;
            this.cdr.markForCheck();
          },
          error: () => {
            this.isLoading = false;
            this.cdr.markForCheck();
          }
        });
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  private handleQueryParams(): void {
    this.route.queryParams.subscribe(params => {
      if (params['doctorId']) {
        this.selectedDoctorId = Number(params['doctorId']);
        this.onDoctorSelect();
      }
      if (params['slotId']) {
        this.selectedSlotId = Number(params['slotId']);
      }
      if (params['date']) {
        this.selectedDate = params['date'];
      }
      if (params['patientId']) {
        this.patientMode = 'existing';
        this.selectedPatientId = Number(params['patientId']);
      }
      this.cdr.markForCheck();
    });
  }

  onDoctorSelect(): void {
    this.selectedDoctor = this.doctors.find(d => d.doctorId === this.selectedDoctorId) || null;
    this.selectedSlotId = 0;
    this.selectedSlot = null;
    this.availableSlots = [];

    if (this.selectedDoctorId > 0) {
      this.loadSlotsForDoctor(this.selectedDoctorId);
    }
    this.cdr.markForCheck();
  }

  loadSlotsForDoctor(docId: number): void {
    this.api.getDoctorSlots(docId, this.selectedDate || undefined, true).subscribe({
      next: (slots) => {
        this.availableSlots = slots;
        if (this.selectedSlotId > 0) {
          this.selectedSlot = this.availableSlots.find(s => s.slotId === this.selectedSlotId) || null;
        }
        this.cdr.markForCheck();
      }
    });
  }

  onSlotSelect(slot: AppointmentSlot): void {
    this.selectedSlotId = slot.slotId;
    this.selectedSlot = slot;
    this.cdr.markForCheck();
  }

  onDateFilterChange(): void {
    if (this.selectedDoctorId > 0) {
      this.loadSlotsForDoctor(this.selectedDoctorId);
    }
  }

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.selectedDoctorId || !this.selectedSlotId || !this.selectedSlot) {
      this.errorMessage = 'Please select a doctor and an available appointment slot.';
      return;
    }

    if (this.patientMode === 'new') {
      if (!this.patientName.trim()) {
        this.errorMessage = 'Please enter patient full name.';
        return;
      }
      if (!this.patientPhone.trim()) {
        this.errorMessage = 'Please enter patient phone number.';
        return;
      }
      if (!this.patientEmail.trim() || !this.patientEmail.includes('@')) {
        this.errorMessage = 'Please enter a valid patient email.';
        return;
      }
    } else {
      if (!this.selectedPatientId || this.selectedPatientId <= 0) {
        this.errorMessage = 'Please select an existing registered patient record.';
        return;
      }
    }

    this.isSubmitting = true;

    const payload = {
      doctorId: this.selectedDoctorId,
      slotId: this.selectedSlotId,
      appointmentDate: this.selectedSlot.slotDate,
      appointmentTime: this.selectedSlot.slotTime,
      patientId: this.patientMode === 'existing' ? this.selectedPatientId : undefined,
      patientName: this.patientMode === 'new' ? this.patientName.trim() : undefined,
      patientPhone: this.patientMode === 'new' ? this.patientPhone.trim() : undefined,
      patientEmail: this.patientMode === 'new' ? this.patientEmail.trim() : undefined,
      patientDateOfBirth: this.patientMode === 'new' && this.patientDateOfBirth ? this.patientDateOfBirth : undefined
    };

    this.api.bookAppointment(payload).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        if (res.success && res.referenceNumber) {
          this.notify.success('Appointment booked successfully!');
          this.router.navigate(['/confirmation', res.referenceNumber]);
        } else {
          this.errorMessage = res.message || 'Booking was not confirmed.';
        }
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isSubmitting = false;
        if (err.status === 409) {
          this.errorMessage = err.error?.message || 'The selected slot was just reserved by another patient. Please choose a different slot.';
          this.loadSlotsForDoctor(this.selectedDoctorId);
        } else {
          this.errorMessage = err.error?.message || 'Server error occurred during booking.';
        }
        this.notify.error(this.errorMessage);
        this.cdr.markForCheck();
      }
    });
  }
}

