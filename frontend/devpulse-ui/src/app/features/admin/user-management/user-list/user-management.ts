import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';
import { TableColumn } from '../../../../core/models/table-column';
import { TableAction } from '../../../../core/models/table-action';
import { JsonPipe } from '@angular/common';
import { UserAccountDto } from '../../../../core/models/user-account.dto';
import { UserApiService } from '../../../../core/services/user-api';
import { LoadingService } from '../../../../core/services/loading-service';
import { Subscription } from 'rxjs';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-management',
  imports: [GenericTableComponent],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss',
})
export class UserManagement implements OnInit, OnDestroy {
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

  readonly actions: TableAction[] = [
    {
      label: '',
      color: '#e4e2e2',
      icon: 'edit',
      action: 'edit',
      tooltip: 'edit',
    },
    {
      label: '',
      color: '#bdb6b6',
      icon: 'transform',
      action: 'activateOrDeactivate',
      tooltip: 'activate / deactivate',
    },
  ];

  readonly pageSizeOptions: number[] = [10, 20, 30, 40, 50, 100];

  users: UserAccountDto[] = [];

  private readonly userApiService: UserApiService = inject(UserApiService);
  private readonly loadingService: LoadingService = inject(LoadingService);
  private readonly snackbarService: SnackbarService = inject(SnackbarService);
  private readonly router: Router = inject(Router);
  private readonly compositeSubscription: Subscription = new Subscription();

  ngOnInit(): void {
    this.fetchAllUserProfiles();
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  handleAction(event: { action: string; row: UserAccountDto }) {
    //alert(`${event.action.toUpperCase()} on ${JSON.stringify(event.row)}`);

    if (event.row.userRole === 'Admin') {
      this.snackbarService.error(
        'Admin user cannot be edited / deactivated. Please contact Dev Team.'
      );
      return;
    }

    if (event.action === 'edit') {
      this.edit(event.row);
    } else if (event.action === 'activateOrDeactivate') {
      this.activateOrDeactivate(event.row);
    }
  }

  private activateOrDeactivate(user: UserAccountDto): void {
    //this.snackbarService.error(`Delete ${user.displayName}`);

    const isDeactivate = user.isActive;
    const confirmQuestion = isDeactivate
      ? `Deactivate user ${user.displayName} ?`
      : `Reactivate user ${user.displayName} ?`;
    this.snackbarService.confirm(confirmQuestion).subscribe((confirmed) => {
      if (confirmed) {
        isDeactivate
          ? this.deactivateUser(user.id)
          : this.activateUser(user.id);
      }
    });
  }

  private activateUser(userId: string): void {
    //alert('activate ' + userId);
    this.loadingService.show();
    const subscription = this.userApiService.restoreUser(userId).subscribe({
      next: () => {
        this.snackbarService.success('User Activated');
        this.loadingService.hide();

        this.fetchAllUserProfiles();
      },
      error: (err) => {
        console.error('Failed user activation', err);
        this.snackbarService.error('Failed user activation !');
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(subscription);
  }

  private deactivateUser(userId: string): void {
    //alert('deactivate ' + userId);
    this.loadingService.show();
    const subscription = this.userApiService.softDeleteUser(userId).subscribe({
      next: () => {
        this.snackbarService.success('User Deactivated');
        this.loadingService.hide();

        this.fetchAllUserProfiles();
      },
      error: (err) => {
        console.error('Failed user deactivation', err);
        this.snackbarService.error('Failed user deactivation !');
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(subscription);
  }

  private edit(user: UserAccountDto): void {
    //this.snackbarService.info(`Editing ${user.displayName}`);
    this.router.navigate(['users/edit/', user.id]);
  }

  private fetchAllUserProfiles(): void {
    this.loadingService.show();
    const subscription = this.userApiService
      .getAllUserProfiles(true)
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
