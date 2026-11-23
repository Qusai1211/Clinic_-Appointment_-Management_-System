import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.less']
})
export class AdminLoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private notify = inject(NotificationService);

  username = '';
  password = '';
  isLoggingIn = false;
  errorMessage = '';

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.username.trim()) {
      this.errorMessage = 'Please enter administrator username.';
      return;
    }
    if (!this.password.trim()) {
      this.errorMessage = 'Please enter password.';
      return;
    }

    this.isLoggingIn = true;

    this.authService.login({
      username: this.username.trim(),
      password: this.password.trim()
    }).subscribe({
      next: (res) => {
        this.isLoggingIn = false;
        if (res.success) {
          this.notify.success(`Welcome back, ${res.adminName || 'Admin'}!`);
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin/dashboard';
          this.router.navigateByUrl(returnUrl);
        } else {
          this.errorMessage = res.message || 'Invalid administrator credentials.';
        }
      },
      error: (err) => {
        this.isLoggingIn = false;
        this.errorMessage = err.error?.message || 'Invalid administrator username or password.';
        this.notify.error(this.errorMessage);
      }
    });
  }
}

