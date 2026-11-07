import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { NotificationService } from '../../../services/notification.service';
import { Doctor, Specialty, DoctorWorkingHours, AppointmentSlot } from '../../../models/clinic.models';

@Component({
  selector: 'app-manage-doctors',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './manage-doctors.component.html',
  styleUrls: ['./manage-doctors.component.less']
})
export class ManageDoctorsComponent implements OnInit {
  private api = inject(ApiService);
  private notify = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  doctors: Doctor[] = [];
  specialties: Specialty[] = [];
  isLoading = true;
  searchQuery = '';

  isDoctorModalOpen = false;
  isEditing = false;
  currentDoctorId = 0;
  doctorForm = {
    fullName: '',
    specialtyId: 0,
    qualifications: '',
    consultationFee: 50,
    isFeatured: 0,
    profileImageUrl: ''
  };

  isScheduleModalOpen = false;
  selectedDoctorForSchedule: Doctor | null = null;
  workingHoursList: DoctorWorkingHours[] = [];
  slotsList: AppointmentSlot[] = [];

  newWorkingHour = {
    dayOfWeek: 'Sunday',
    startTime: '09:00 AM',
    endTime: '05:00 PM'
  };

  editingWorkingHoursId: number | null = null;
  editWorkingHourForm = {
    dayOfWeek: 'Sunday',
    startTime: '09:00 AM',
    endTime: '05:00 PM'
  };

  newSlot = {
    slotDate: '',
    slotTime: '09:00 AM'
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.api.getSpecialties().subscribe({
      next: (specs) => {
        this.specialties = specs;
        this.cdr.markForCheck();
      }
    });

    this.api.getDoctors(this.searchQuery).subscribe({
      next: (docs) => {
        this.doctors = docs;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.notify.error('Failed to load doctors.');
        this.cdr.markForCheck();
      }
    });
  }

  openCreateModal(): void {
    this.isEditing = false;
    this.currentDoctorId = 0;
    this.doctorForm = {
      fullName: '',
      specialtyId: this.specialties.length > 0 ? this.specialties[0].specialtyId : 0,
      qualifications: '',
      consultationFee: 60,
      isFeatured: 0,
      profileImageUrl: ''
    };
    this.isDoctorModalOpen = true;
    this.cdr.markForCheck();
  }

  openEditModal(doc: Doctor): void {
    this.isEditing = true;
    this.currentDoctorId = doc.doctorId;
    this.doctorForm = {
      fullName: doc.fullName,
      specialtyId: doc.specialtyId || 0,
      qualifications: doc.qualifications || '',
      consultationFee: doc.consultationFee,
      isFeatured: doc.isFeatured,
      profileImageUrl: doc.profileImageUrl || ''
    };
    this.isDoctorModalOpen = true;
    this.cdr.markForCheck();
  }

  closeDoctorModal(): void {
    this.isDoctorModalOpen = false;
    this.cdr.markForCheck();
  }

  saveDoctor(): void {
    if (!this.doctorForm.fullName.trim()) {
      this.notify.warning('Please enter doctor name.');
      return;
    }

    const payload = {
      fullName: this.doctorForm.fullName.trim(),
      specialtyId: this.doctorForm.specialtyId > 0 ? this.doctorForm.specialtyId : undefined,
      qualifications: this.doctorForm.qualifications.trim(),
      consultationFee: Number(this.doctorForm.consultationFee),
      isFeatured: Number(this.doctorForm.isFeatured),
      profileImageUrl: this.doctorForm.profileImageUrl.trim()
    };

    if (this.isEditing) {
      this.api.updateDoctor(this.currentDoctorId, payload).subscribe({
        next: () => {
          this.notify.success('Doctor updated successfully in Oracle.');
          this.closeDoctorModal();
          this.loadData();
        },
        error: (err) => {
          this.notify.error(err.error?.message || 'Update failed.');
          this.cdr.markForCheck();
        }
      });
    } else {
      this.api.createDoctor(payload).subscribe({
        next: () => {
          this.notify.success('Doctor created successfully in Oracle.');
          this.closeDoctorModal();
          this.loadData();
        },
        error: (err) => {
          this.notify.error(err.error?.message || 'Creation failed.');
          this.cdr.markForCheck();
        }
      });
    }
  }

  deleteDoctor(doc: Doctor): void {
    if (confirm(`Are you sure you want to delete ${doc.fullName}?`)) {
      this.api.deleteDoctor(doc.doctorId).subscribe({
        next: () => {
          this.notify.success('Doctor deleted successfully.');
          this.loadData();
        },
        error: (err) => {
          this.notify.error(err.error?.message || 'Delete failed.');
          this.cdr.markForCheck();
        }
      });
    }
  }

  openScheduleModal(doc: Doctor): void {
    this.selectedDoctorForSchedule = doc;
    this.loadDoctorSchedule(doc.doctorId);
    this.isScheduleModalOpen = true;
    this.cdr.markForCheck();
  }

  closeScheduleModal(): void {
    this.isScheduleModalOpen = false;
    this.selectedDoctorForSchedule = null;
    this.cdr.markForCheck();
  }

  loadDoctorSchedule(doctorId: number): void {
    this.api.getDoctorWorkingHours(doctorId).subscribe({
      next: (hours) => {
        this.workingHoursList = hours;
        this.cdr.markForCheck();
      }
    });

    this.api.getDoctorSlots(doctorId).subscribe({
      next: (slots) => {
        this.slotsList = slots;
        this.cdr.markForCheck();
      }
    });
  }

  addWorkingHours(): void {
    if (!this.selectedDoctorForSchedule) return;

    this.api.addDoctorWorkingHours(this.selectedDoctorForSchedule.doctorId, {
      dayOfWeek: this.newWorkingHour.dayOfWeek,
      startTime: this.newWorkingHour.startTime,
      endTime: this.newWorkingHour.endTime
    }).subscribe({
      next: () => {
        this.notify.success('Working hour added.');
        this.loadDoctorSchedule(this.selectedDoctorForSchedule!.doctorId);
      },
      error: () => {
        this.notify.error('Failed to add working hour.');
        this.cdr.markForCheck();
      }
    });
  }

  deleteWorkingHours(id: number): void {
    if (!this.selectedDoctorForSchedule) return;

    this.api.deleteDoctorWorkingHours(id).subscribe({
      next: () => {
        this.notify.success('Working hour removed.');
        this.loadDoctorSchedule(this.selectedDoctorForSchedule!.doctorId);
      }
    });
  }

  addSlot(): void {
    if (!this.selectedDoctorForSchedule) return;
    if (!this.newSlot.slotDate) {
      this.notify.warning('Please select a date for the slot.');
      return;
    }

    this.api.addDoctorSlot(this.selectedDoctorForSchedule.doctorId, {
      slotDate: this.newSlot.slotDate,
      slotTime: this.newSlot.slotTime
    }).subscribe({
      next: () => {
        this.notify.success('Appointment slot added.');
        this.loadDoctorSchedule(this.selectedDoctorForSchedule!.doctorId);
      },
      error: () => {
        this.notify.error('Failed to add slot.');
        this.cdr.markForCheck();
      }
    });
  }

  startEditWorkingHours(h: DoctorWorkingHours): void {
    this.editingWorkingHoursId = h.workingHoursId;
    this.editWorkingHourForm = {
      dayOfWeek: h.dayOfWeek,
      startTime: h.startTime,
      endTime: h.endTime
    };
    this.cdr.markForCheck();
  }

  cancelEditWorkingHours(): void {
    this.editingWorkingHoursId = null;
    this.cdr.markForCheck();
  }

  saveWorkingHoursEdit(): void {
    if (!this.editingWorkingHoursId || !this.selectedDoctorForSchedule) return;

    this.api.updateDoctorWorkingHours(this.editingWorkingHoursId, {
      dayOfWeek: this.editWorkingHourForm.dayOfWeek,
      startTime: this.editWorkingHourForm.startTime,
      endTime: this.editWorkingHourForm.endTime
    }).subscribe({
      next: () => {
        this.notify.success('Working hour schedule updated.');
        this.editingWorkingHoursId = null;
        this.loadDoctorSchedule(this.selectedDoctorForSchedule!.doctorId);
      },
      error: (err) => {
        this.notify.error(err.error?.message || 'Failed to update working hours.');
        this.cdr.markForCheck();
      }
    });
  }

  toggleSlotAvailability(slot: AppointmentSlot): void {
    if (!this.selectedDoctorForSchedule) return;

    const newStatus = slot.isAvailable === 0;
    this.api.updateDoctorSlotAvailability(slot.slotId, newStatus).subscribe({
      next: () => {
        this.notify.success(`Slot marked as ${newStatus ? 'Available' : 'Unavailable'}.`);
        this.loadDoctorSchedule(this.selectedDoctorForSchedule!.doctorId);
      },
      error: (err) => {
        this.notify.error(err.error?.message || 'Failed to update slot status.');
        this.cdr.markForCheck();
      }
    });
  }

  deleteSlot(slotId: number): void {
    if (!this.selectedDoctorForSchedule) return;

    this.api.deleteDoctorSlot(slotId).subscribe({
      next: () => {
        this.notify.success('Slot removed.');
        this.loadDoctorSchedule(this.selectedDoctorForSchedule!.doctorId);
      }
    });
  }
}

