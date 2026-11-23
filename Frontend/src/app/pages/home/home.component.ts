import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Doctor, Specialty } from '../../models/clinic.models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  specialties: Specialty[] = [];
  featuredDoctors: Doctor[] = [];
  searchQuery: string = '';
  selectedSpecialtyId: number = 0;
  isLoading = true;

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

    this.api.getFeaturedDoctors().subscribe({
      next: (docs) => {
        this.featuredDoctors = docs;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onSearch(): void {
    const queryParams: any = {};
    if (this.searchQuery.trim()) {
      queryParams.search = this.searchQuery.trim();
    }
    if (this.selectedSpecialtyId > 0) {
      queryParams.specialty = this.selectedSpecialtyId;
    }
    this.router.navigate(['/doctors'], { queryParams });
  }

  goToSpecialty(specialtyId: number): void {
    this.router.navigate(['/doctors'], { queryParams: { specialty: specialtyId } });
  }
}

