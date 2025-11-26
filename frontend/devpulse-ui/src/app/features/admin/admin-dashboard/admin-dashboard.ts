import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { OrchestratorApiService } from '../../../core/services/orchestrator-api';
import { AdminDashboardDto } from '../../../core/models/base-dashbaord.dto';
import { UserStoreService } from '../../../core/services/user-store.service';
import { UserAccountDto } from '../../../core/models/user-account.dto';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../core/services/loading-service';

@Component({
  selector: 'app-admin-dashboard',
  imports: [],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss',
})
export class AdminDashboard implements OnInit, OnDestroy {
  private readonly compositeSubscription: Subscription = new Subscription();

  private loadingService = inject(LoadingService);
  private userStoreService = inject(UserStoreService);
  private orchestratorApiService = inject(OrchestratorApiService);

  private userDto: UserAccountDto | null = null;
  adminDashBoard: AdminDashboardDto | null = null;

  ngOnInit(): void {
    this.loadingService.show();
    this.userDto = this.userStoreService.userDto();
    if (this.userDto) {
      const sub = this.orchestratorApiService
        .getAdminDashBaord(this.userDto.id)
        .subscribe({
          next: (value: AdminDashboardDto) => {
            this.adminDashBoard = value;
            console.log('AdminDashboardDto', this.adminDashBoard);
            this.loadingService.hide();
          },
          error: (err) => {
            console.error('Failed to load admin dashboard', err);
            this.loadingService.hide();
          },
        });

      this.compositeSubscription.add(sub);
    }
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }
}
