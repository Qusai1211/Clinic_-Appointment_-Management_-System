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
}
