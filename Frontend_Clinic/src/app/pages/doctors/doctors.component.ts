import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Doctor, Specialty } from '../../models/clinic.models';

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.less']
})
export class DoctorsComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);

  doctors: Doctor[] = [];
  specialties: Specialty[] = [];

  searchQuery = '';
  selectedSpecialtyId = 0;
  sortBy = 'name_asc';
  isLoading = true;

  ngOnInit(): void {
    this.api.getSpecialties().subscribe({
      next: (specs) => {
        this.specialties = specs;
        this.cdr.markForCheck();
      }
    });

    this.route.queryParams.subscribe(params => {
      if (params['search']) {
        this.searchQuery = params['search'];
      }
      if (params['specialty']) {
        this.selectedSpecialtyId = Number(params['specialty']);
      }
      this.loadDoctors();
    });
  }

  loadDoctors(): void {
    this.isLoading = true;
    const specId = this.selectedSpecialtyId > 0 ? this.selectedSpecialtyId : undefined;
    this.api.getDoctors(this.searchQuery, specId, this.sortBy).subscribe({
      next: (docs) => {
        this.doctors = docs;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onFilterChange(): void {
    this.loadDoctors();
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.selectedSpecialtyId = 0;
    this.sortBy = 'name_asc';
    this.loadDoctors();
  }
}

