import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Appointment } from '../../models/clinic.models';

@Component({
  selector: 'app-appointment-confirmation',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './appointment-confirmation.component.html',
  styleUrls: ['./appointment-confirmation.component.less']
})
export class AppointmentConfirmationComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private api = inject(ApiService);
  private cdr = inject(ChangeDetectorRef);

  referenceNumber: string = '';
  appointment: Appointment | null = null;
  isLoading = true;
  errorMessage = '';
  copied = false;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const ref = params.get('referenceNumber');
      if (ref) {
        this.referenceNumber = ref;
        this.loadAppointment(ref);
      }
    });
  }

  loadAppointment(ref: string): void {
    this.isLoading = true;
    this.api.getAppointmentByReference(ref).subscribe({
      next: (app) => {
        this.appointment = app;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Appointment reference not found in the database.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  copyReference(): void {
    if (this.referenceNumber) {
      navigator.clipboard.writeText(this.referenceNumber);
      this.copied = true;
      setTimeout(() => {
        this.copied = false;
        this.cdr.markForCheck();
      }, 2500);
      this.cdr.markForCheck();
    }
  }

  printConfirmation(): void {
    window.print();
  }
}
