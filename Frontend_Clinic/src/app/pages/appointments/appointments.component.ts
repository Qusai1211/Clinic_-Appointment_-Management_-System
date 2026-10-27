import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { NotificationService } from '../../services/notification.service';
import { Appointment } from '../../models/clinic.models';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.less']
})
export class AppointmentsComponent implements OnInit {
  private api = inject(ApiService);
  private notify = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  appointments: Appointment[] = [];
  searchQuery = '';
  filterStatus = 'all';
  filterDate = '';
  isLoading = true;

  // Cancellation Modal state
  appointmentToCancel: Appointment | null = null;
  isCancelling = false;

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
        this.notify.error('Failed to load appointments from server.');
        this.cdr.markForCheck();
      }
    });
  }

  onFilterChange(): void {
    this.loadAppointments();
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.filterStatus = 'all';
    this.filterDate = '';
    this.loadAppointments();
  }

  openCancelModal(app: Appointment): void {
    this.appointmentToCancel = app;
    this.cdr.markForCheck();
  }

  closeCancelModal(): void {
    this.appointmentToCancel = null;
    this.cdr.markForCheck();
  }

  confirmCancellation(): void {
    if (!this.appointmentToCancel) return;

    this.isCancelling = true;
    this.api.cancelAppointment(this.appointmentToCancel.appointmentId).subscribe({
      next: () => {
        this.isCancelling = false;
        this.notify.success(`Appointment ${this.appointmentToCancel?.referenceNumber} cancelled successfully.`);
        this.closeCancelModal();
        this.loadAppointments();
      },
      error: (err) => {
        this.isCancelling = false;
        const msg = err.error?.message || 'Failed to cancel appointment.';
        this.notify.error(msg);
        this.cdr.markForCheck();
      }
    });
  }
}

