import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { NotificationService } from '../../../services/notification.service';
import { Patient } from '../../../models/clinic.models';

@Component({
  selector: 'app-manage-patients',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './manage-patients.component.html',
  styleUrls: ['./manage-patients.component.less']
})
export class ManagePatientsComponent implements OnInit {
  private api = inject(ApiService);
  private notify = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  patients: Patient[] = [];
  searchQuery = '';
  isLoading = true;

  selectedPatient: Patient | null = null;

  isEditModalOpen = false;
  editForm = {
    patientId: 0,
    fullName: '',
    phoneNumber: '',
    email: '',
    dateOfBirth: ''
  };

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this.isLoading = true;
    this.api.getPatients(this.searchQuery).subscribe({
      next: (data) => {
        this.patients = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.notify.error('Failed to load patient records.');
        this.cdr.markForCheck();
      }
    });
  }

  viewPatient(p: Patient): void {
    this.selectedPatient = p;
    this.cdr.markForCheck();
  }

  closeModal(): void {
    this.selectedPatient = null;
    this.cdr.markForCheck();
  }

  openEditModal(p: Patient): void {
    const dobStr = p.dateOfBirth ? new Date(p.dateOfBirth).toISOString().split('T')[0] : '';
    this.editForm = {
      patientId: p.patientId,
      fullName: p.fullName,
      phoneNumber: p.phoneNumber,
      email: p.email,
      dateOfBirth: dobStr
    };
    this.isEditModalOpen = true;
    this.cdr.markForCheck();
  }

  closeEditModal(): void {
    this.isEditModalOpen = false;
    this.cdr.markForCheck();
  }

  savePatientEdit(): void {
    if (!this.editForm.fullName.trim() || !this.editForm.phoneNumber.trim()) {
      this.notify.warning('Full name and phone number are required.');
      return;
    }

    this.api.updatePatient(this.editForm.patientId, {
      fullName: this.editForm.fullName.trim(),
      phoneNumber: this.editForm.phoneNumber.trim(),
      email: this.editForm.email.trim(),
      dateOfBirth: this.editForm.dateOfBirth
    }).subscribe({
      next: () => {
        this.notify.success('Patient record updated successfully.');
        this.closeEditModal();
        this.loadPatients();
      },
      error: (err) => {
        this.notify.error(err.error?.message || 'Failed to update patient record.');
        this.cdr.markForCheck();
      }
    });
  }

  deletePatient(p: Patient): void {
    if (confirm(`Are you sure you want to delete patient "${p.fullName}"? Associated appointments will be cascade-removed.`)) {
      this.api.deletePatient(p.patientId).subscribe({
        next: () => {
          this.notify.success('Patient record deleted successfully.');
          this.loadPatients();
        },
        error: (err) => {
          this.notify.error(err.error?.message || 'Failed to delete patient record.');
          this.cdr.markForCheck();
        }
      });
    }
  }
}
