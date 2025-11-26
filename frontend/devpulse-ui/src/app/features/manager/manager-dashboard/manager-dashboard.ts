import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { OrchestratorApiService } from '../../../core/services/orchestrator-api';
import { Subscription } from 'rxjs';
import { ManagerDashboardDto } from '../../../core/models/base-dashbaord.dto';
import { UserAccountDto } from '../../../core/models/user-account.dto';
import { UserStoreService } from '../../../core/services/user-store.service';
import { LoadingService } from '../../../core/services/loading-service';

@Component({
  selector: 'app-manager-dashboard',
  imports: [],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.scss',
})
export class ManagerDashboard implements OnInit, OnDestroy {
  private readonly compositeSubscription: Subscription = new Subscription();

  private loadingService = inject(LoadingService);
  private userStoreService = inject(UserStoreService);
  private orchestratorApiService = inject(OrchestratorApiService);

  private userDto: UserAccountDto | null = null;
  managerDashBoard: ManagerDashboardDto | null = null;

  ngOnInit(): void {
    this.loadingService.show();
    this.userDto = this.userStoreService.userDto();
    if (this.userDto) {
      const sub = this.orchestratorApiService
        .getAdminDashBaord(this.userDto.id)
        .subscribe({
          next: (value: ManagerDashboardDto) => {
            this.managerDashBoard = value;
            console.log('ManagerDashboardDto', this.managerDashBoard);
            this.loadingService.hide();
          },
          error: (err) => {
            console.error('Failed to load manager dashboard', err);
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
