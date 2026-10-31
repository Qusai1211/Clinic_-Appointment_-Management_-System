import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { NotificationService } from '../../../services/notification.service';
import { Specialty } from '../../../models/clinic.models';

@Component({
  selector: 'app-manage-specialties',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './manage-specialties.component.html',
  styleUrls: ['./manage-specialties.component.less']
})
export class ManageSpecialtiesComponent implements OnInit {
  private api = inject(ApiService);
  private notify = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  specialties: Specialty[] = [];
  isLoading = true;

  isModalOpen = false;
  isEditing = false;
  currentSpecialtyId = 0;
  form = {
    specialtyName: '',
    description: ''
  };

  ngOnInit(): void {
    this.loadSpecialties();
  }

  loadSpecialties(): void {
    this.isLoading = true;
    this.api.getSpecialties().subscribe({
      next: (specs) => {
        this.specialties = specs;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.notify.error('Failed to load specialties.');
        this.cdr.markForCheck();
      }
    });
  }

  openCreateModal(): void {
    this.isEditing = false;
    this.currentSpecialtyId = 0;
    this.form = { specialtyName: '', description: '' };
    this.isModalOpen = true;
    this.cdr.markForCheck();
  }

  openEditModal(s: Specialty): void {
    this.isEditing = true;
    this.currentSpecialtyId = s.specialtyId;
    this.form = {
      specialtyName: s.specialtyName,
      description: s.description || ''
    };
    this.isModalOpen = true;
    this.cdr.markForCheck();
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.cdr.markForCheck();
  }

  saveSpecialty(): void {
    if (!this.form.specialtyName.trim()) {
      this.notify.warning('Please enter specialty name.');
      return;
    }

    const payload = {
      specialtyName: this.form.specialtyName.trim(),
      description: this.form.description.trim()
    };

    if (this.isEditing) {
      this.api.updateSpecialty(this.currentSpecialtyId, payload).subscribe({
        next: () => {
          this.notify.success('Specialty updated successfully.');
          this.closeModal();
          this.loadSpecialties();
        },
        error: (err) => {
          this.notify.error(err.error?.message || 'Update failed.');
          this.cdr.markForCheck();
        }
      });
    } else {
      this.api.createSpecialty(payload).subscribe({
        next: () => {
          this.notify.success('Specialty created successfully.');
          this.closeModal();
          this.loadSpecialties();
        },
        error: (err) => {
          this.notify.error(err.error?.message || 'Create failed.');
          this.cdr.markForCheck();
        }
      });
    }
  }

  deleteSpecialty(s: Specialty): void {
    if (confirm(`Are you sure you want to delete ${s.specialtyName}?`)) {
      this.api.deleteSpecialty(s.specialtyId).subscribe({
        next: () => {
          this.notify.success('Specialty deleted successfully.');
          this.loadSpecialties();
        },
        error: (err) => {
          this.notify.error(err.error?.message || 'Delete failed.');
          this.cdr.markForCheck();
        }
      });
    }
  }
}

