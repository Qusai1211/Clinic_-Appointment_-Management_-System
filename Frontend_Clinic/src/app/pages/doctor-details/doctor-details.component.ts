import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Doctor, AppointmentSlot, DoctorWorkingHours } from '../../models/clinic.models';

@Component({
  selector: 'app-doctor-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './doctor-details.component.html',
  styleUrls: ['./doctor-details.component.less']
})
export class DoctorDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(ApiService);
  private cdr = inject(ChangeDetectorRef);

  doctorId: number = 0;
  doctor: Doctor | null = null;
  workingHours: DoctorWorkingHours[] = [];
  availableSlots: AppointmentSlot[] = [];

  selectedSlot: AppointmentSlot | null = null;
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.doctorId = Number(id);
        this.loadDoctorDetails();
      }
    });
  }

  loadDoctorDetails(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.api.getDoctor(this.doctorId).subscribe({
      next: (doc) => {
        this.doctor = doc;
        this.workingHours = doc.workingHours || [];
        this.availableSlots = doc.availableSlots || [];
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Doctor profile could not be loaded.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  selectSlot(slot: AppointmentSlot): void {
    this.selectedSlot = slot;
  }

  proceedToBooking(): void {
    if (!this.selectedSlot || !this.doctor) return;

    this.router.navigate(['/book'], {
      queryParams: {
        doctorId: this.doctor.doctorId,
        slotId: this.selectedSlot.slotId,
        date: this.selectedSlot.slotDate,
        time: this.selectedSlot.slotTime
      }
    });
  }
}
