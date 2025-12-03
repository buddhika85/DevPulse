import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { OrchestratorApiService } from '../../../core/services/orchestrator-api';
import { UserDashboardDto } from '../../../core/models/base-dashbaord.dto';
import { UserAccountDto } from '../../../core/models/user-account.dto';
import { UserStoreService } from '../../../core/services/user-store.service';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../core/services/loading-service';

@Component({
  selector: 'app-developer-dashboard',
  imports: [],
  templateUrl: './developer-dashboard.html',
  styleUrl: './developer-dashboard.scss',
})
export class DeveloperDashboard implements OnInit, OnDestroy {
  private readonly compositeSubscription: Subscription = new Subscription();

  private readonly loadingService = inject(LoadingService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly orchestratorApiService = inject(OrchestratorApiService);

  private userDto: UserAccountDto | null = null;
  userDashBoard: UserDashboardDto | null = null;

  ngOnInit(): void {
    this.loadingService.show();
    this.userDto = this.userStoreService.userDto();
    if (this.userDto) {
      const sub = this.orchestratorApiService
        .getDeveloperDashBaord(this.userDto.id)
        .subscribe({
          next: (value: UserDashboardDto) => {
            this.userDashBoard = value;
            console.log('UserDashboardDto', this.userDashBoard);
            this.loadingService.hide();
          },
          error: (err) => {
            console.error('Failed to load developer/user dashboard', err);
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
