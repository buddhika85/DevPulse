import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth';
import { AccountInfo } from '@azure/msal-browser';
import { CommonModule } from '@angular/common';
import { UserAccountDto, UserApiService } from '../../core/services/user-api';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatProgressSpinnerModule],

  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell implements OnInit {
  user: AccountInfo | null = null; // Entra External Id user
  userDto: UserAccountDto | null = null;
  isLoading = false;

  constructor(
    private authService: AuthService,
    private userApi: UserApiService
  ) {}

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.isLoading = true;
      this.user = this.authService.getUser();
      this.userApi.getUserProfile().subscribe({
        next: (profile) => {
          this.userDto = profile;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Failed to load user profile', err);
          this.isLoading = false;
        },
      });
    }
  }

  login(): void {
    this.authService.login();
  }

  logout(): void {
    this.authService.logout();
  }
}
