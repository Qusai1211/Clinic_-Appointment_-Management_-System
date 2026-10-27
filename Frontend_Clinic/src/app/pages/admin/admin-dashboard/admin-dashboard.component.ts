import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { AdminDashboardStats, SpecialtyAppointmentChartItem } from '../../../models/clinic.models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.less']
})
export class AdminDashboardComponent implements OnInit {
  private api = inject(ApiService);
  private cdr = inject(ChangeDetectorRef);
  authService = inject(AuthService);

  stats: AdminDashboardStats = {
    totalDoctors: 0,
    totalPatients: 0,
    totalAppointments: 0,
    scheduledAppointments: 0,
    completedAppointments: 0,
    cancelledAppointments: 0
  };

  chartData: SpecialtyAppointmentChartItem[] = [];
  maxAppointmentCount = 1;
  isLoading = true;

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;

    this.api.getDashboardStats().subscribe({
      next: (s) => {
        this.stats = s;
        this.cdr.markForCheck();
      }
    });

    this.api.getSpecialtyChartData().subscribe({
      next: (data) => {
        this.chartData = data;
        const max = Math.max(...data.map(d => d.appointmentCount), 1);
        this.maxAppointmentCount = max;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  getBarHeightPercentage(count: number): number {
    return Math.max(Math.round((count / this.maxAppointmentCount) * 100), 4);
  }
}
