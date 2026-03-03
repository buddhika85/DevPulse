import { Component, inject } from '@angular/core';
import { TableColumn } from '../../../../core/models/table-column';
import { TableAction } from '../../../../core/models/table-action';
import { UserAccountDto } from '../../../../core/models/user-account.dto';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { UserApiService } from '../../../../core/services/user-api';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';

@Component({
  selector: 'app-team-dashboard',
  imports: [GenericTableComponent],
  templateUrl: './team-dashboard.html',
  styleUrl: './team-dashboard.scss',
})
export class TeamDashboard {
  readonly columns: TableColumn[] = [
    { key: 'id', label: 'ID' },
    { key: 'displayName', label: 'Name' },
    { key: 'email', label: 'Username / Email' },
    { key: 'userRole', label: 'Role' },
    { key: 'createdAt', label: 'Created' },
    // { key: 'managerId', label: 'Manager ID' },
    { key: 'managerName', label: 'Manager Name' },
    { key: 'isActiveStr', label: 'Is Active?' },
  ];

  readonly actions: TableAction[] = [];

  readonly pageSizeOptions: number[] = [10, 20, 30, 40, 50, 100];

  users: UserAccountDto[] = [];

  private readonly userApiService: UserApiService = inject(UserApiService);
  private readonly loadingService: LoadingService = inject(LoadingService);
  private readonly snackbarService: SnackbarService = inject(SnackbarService);
  private readonly router: Router = inject(Router);
  private readonly compositeSubscription: Subscription = new Subscription();

  ngOnInit(): void {
    this.fetchTeamProfiles();
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  handleAction(event: { action: string; row: UserAccountDto }) {
    // there is no action to handle as its a read only table/list of users for manager
    // keeping this to satisfy child component - app-generic-table
  }

  fetchTeamProfiles() {
    this.loadingService.show();
    const subscription = this.userApiService
      .getTeamMembersForManager(true)
      .subscribe({
        next: (users: UserAccountDto[]) => {
          this.users = users;
          //console.log('users list: ', this.users);
          this.loadingService.hide();
        },
        error: (err) => {
          console.error('Failed to fetch all user profiles', err);
          this.snackbarService.error('Failed to fetch all user profiles !');
          this.loadingService.hide();
        },
      });
    this.compositeSubscription.add(subscription);
  }
}
