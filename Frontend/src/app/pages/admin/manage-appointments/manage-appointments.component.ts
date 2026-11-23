import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { NotificationService } from '../../../services/notification.service';
import { Appointment } from '../../../models/clinic.models';

@Component({
  selector: 'app-manage-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './manage-appointments.component.html',
  styleUrls: ['./manage-appointments.component.less']
})
export class ManageAppointmentsComponent implements OnInit {
  private api = inject(ApiService);
  private notify = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  appointments: Appointment[] = [];
  searchQuery = '';
  filterStatus = 'all';
  filterDate = '';
  isLoading = true;

  selectedAppForStatus: Appointment | null = null;
  newStatus = 'Scheduled';
  isUpdating = false;

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.isLoading = true;
    const dateParam = this.filterDate || undefined;
    const statusParam = this.filterStatus !== 'all' ? this.filterStatus : undefined;

    this.api.getAppointments(this.searchQuery, dateParam, statusParam).subscribe({
      next: (apps) => {
        this.appointments = apps;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.notify.error('Failed to load appointments.');
        this.cdr.markForCheck();
      }
    });
  }

  openStatusModal(app: Appointment): void {
    this.selectedAppForStatus = app;
    this.newStatus = app.status;
    this.cdr.markForCheck();
  }

  closeStatusModal(): void {
    this.selectedAppForStatus = null;
    this.cdr.markForCheck();
  }

  saveStatus(): void {
    if (!this.selectedAppForStatus) return;

    this.isUpdating = true;
    this.api.updateAppointmentStatus(this.selectedAppForStatus.appointmentId, this.newStatus).subscribe({
      next: () => {
        this.isUpdating = false;
        this.notify.success(`Status updated to '${this.newStatus}' for appointment ${this.selectedAppForStatus?.referenceNumber}.`);
        this.closeStatusModal();
        this.loadAppointments();
      },
      error: (err) => {
        this.isUpdating = false;
        this.notify.error(err.error?.message || 'Failed to update appointment status.');
        this.cdr.markForCheck();
      }
    });
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.filterStatus = 'all';
    this.filterDate = '';
    this.loadAppointments();
  }
}

