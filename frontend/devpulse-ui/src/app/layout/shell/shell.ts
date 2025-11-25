import { Component, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth';
import { AccountInfo } from '@azure/msal-browser';
import { CommonModule } from '@angular/common';
import { UserApiService } from '../../core/services/user-api';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MsalService } from '@azure/msal-angular';
import { UserAccountDto } from '../../core/models/user-account.dto';
import { UserStoreService } from '../../core/services/user-store.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet,
    CommonModule,
    MatProgressSpinnerModule,
    RouterLink,
    RouterLinkActive,
  ],

  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell implements OnInit {
  user = signal<AccountInfo | null>(null); // Entra External Id user
  userDto = signal<UserAccountDto | null>(null);
  isLoading = signal(false);

  constructor(
    private userStoreService: UserStoreService,
    private authService: AuthService,
    private userApi: UserApiService,
    private msal: MsalService
  ) {}

  // ✅ Called on app load or after redirect login
  ngOnInit(): void {
    this.msal.instance.handleRedirectPromise().then((result) => {
      // Make sure MSAL finishes processing the redirect
      if (result) {
        // This gets executed if a login was performed
        this.msal.instance.setActiveAccount(result.account);
      }

      // ✅ Now it's safe to check login state and load user
      this.getUserDetails();
    });
  }

  login(): void {
    this.authService.login();
  }

  logout(): void {
    this.authService.logout(); // ✅ Clear MSAL session (Entra token + account info)
    this.user.set(null);
    this.userDto.set(null);
    localStorage.removeItem('devpulse_token');
  }

  private getUserDetails() {
    if (this.authService.isLoggedIn()) {
      this.isLoading.set(true);

      // set active user received from Entra ID
      this.user.set(this.authService.getUser());

      // getting backend user meta data
      this.userApi.getUserProfile().subscribe({
        next: (userProfileResponseDto) => {
          console.log('User-Profile-Response-Dto', userProfileResponseDto);
          this.userDto.set(userProfileResponseDto.user);
          this.userStoreService.setUserDto(userProfileResponseDto.user);

          // store Dev Pulse JwToken in local storage
          localStorage.setItem(
            'devpulse_token',
            userProfileResponseDto.devPulseJwToken
          );

          this.isLoading.set(false);

          console.log(this.userDto());
        },
        error: (err) => {
          console.error('Failed to load user profile', err);

          this.userDto.set(null);
          this.userStoreService.setUserDto(null);
          this.isLoading.set(false);
        },
      });
    }
  }
}
