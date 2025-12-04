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

@Component({
  selector: 'app-user-management',
  imports: [GenericTableComponent],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss',
})
export class UserManagement implements OnInit, OnDestroy {
  readonly columns: TableColumn[] = [
    // { key: 'id', label: 'ID' },
    { key: 'displayName', label: 'Name' },
    { key: 'email', label: 'Username / Email' },
    { key: 'userRole', label: 'Role' },
    { key: 'createdAt', label: 'Created' },
    // { key: 'managerId', label: 'Manager ID' },
    { key: 'managerName', label: 'Manager Name' },
  ];

  readonly actions: TableAction[] = [
    { label: 'Edit', color: 'accent', icon: 'edit', action: 'edit' },
    { label: 'Delete', color: 'warn', icon: 'delete', action: 'delete' },
  ];

  users: UserAccountDto[] = [];

  private readonly userApiService: UserApiService = inject(UserApiService);
  private readonly loadingService: LoadingService = inject(LoadingService);
  private readonly snackbarService: SnackbarService = inject(SnackbarService);
  private readonly compositeSubscription: Subscription = new Subscription();

  ngOnInit(): void {
    this.fetchAllUserProfiles();
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  handleAction(event: { action: string; row: UserAccountDto }) {
    //alert(`${event.action.toUpperCase()} on ${JSON.stringify(event.row)}`);

    if (event.action === 'edit') {
      this.snackbarService.info(`Editing ${event.row.displayName}`);
    } else if (event.action === 'delete') {
      this.snackbarService.error(`Deleted ${event.row.displayName}`);
    }
  }

  private fetchAllUserProfiles(): void {
    this.loadingService.show();
    const subscription = this.userApiService.getAllUserProfiles().subscribe({
      next: (users: UserAccountDto[]) => {
        this.users = users;
        console.log('users list: ', this.users);
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

//  taskColumns: TableColumn[] = [
//     { key: 'id', label: 'ID' },
//     { key: 'title', label: 'Title' },
//     { key: 'status', label: 'Status' },
//   ];

//   taskActions: TableAction[] = [
//     { label: 'Edit', color: 'accent', icon: 'edit', action: 'edit' },
//     { label: 'Delete', color: 'warn', icon: 'delete', action: 'delete' },
//   ];

//   tasks = [
//     { id: 1, title: 'Write blog post', status: 'Open' },
//     { id: 2, title: 'Fix bug #123', status: 'In Progress' },
//     { id: 3, title: 'Write blog post', status: 'Open' },
//     {
//       id: 4,
//       title:
//         "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
//       status: 'In Progress',
//     },
//     { id: 5, title: 'Write blog post', status: 'Open' },
//     { id: 6, title: 'Fix bug #123', status: 'In Progress' },
//   ];

//   handleTaskAction(event: { action: string; row: any }) {
//     alert(`Action Triggered ${event.action} on ${JSON.stringify(event.row)}`);
//   }
