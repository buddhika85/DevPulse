import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TaskApiService } from '../../../../core/services/task-api';
import { Subscription } from 'rxjs';
import { TaskItemDto } from '../../../../core/models/task-item.dto';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { LoadingService } from '../../../../core/services/loading-service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { TableColumn } from '../../../../core/models/table-column';
import { TableAction } from '../../../../core/models/table-action';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';

@Component({
  selector: 'app-task-list',
  imports: [GenericTableComponent],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss',
})
export class TaskList implements OnInit {
  readonly columns: TableColumn[] = [
    // { key: 'id', label: 'ID' },
    { key: 'title', label: 'Title' },
    { key: 'description', label: 'Description' },
    { key: 'status', label: 'Status' },
    { key: 'createdAtStr', label: 'Created' },

    { key: 'priority', label: 'Priority' },
    { key: 'isDeletedStr', label: 'Is Deleted?' },
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

  private userId!: string;

  private readonly router: Router = inject(Router);
  private readonly taskApi = inject(TaskApiService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly loadingService = inject(LoadingService);
  private readonly compositeSubscription: Subscription = new Subscription();
  private readonly snackbarService: SnackbarService = inject(SnackbarService);

  tasksOfUser: TaskItemDto[] = [];

  ngOnInit(): void {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    this.loadingService.show();
    if (userId) {
      this.userId = userId;
      this.fetchAllUserTasks();
    }
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  handleAction(event: { action: string; row: TaskItemDto }) {
    if (event.action === 'edit') {
      this.edit(event.row);
    } else if (event.action === 'activateOrDeactivate') {
      this.restoreOrSoftDelete(event.row);
    }
  }

  add(): void {
    this.router.navigate(['tasks/add']);
  }

  edit(task: TaskItemDto): void {
    this.router.navigate(['tasks/edit', task.id]);
  }

  private restoreOrSoftDelete(task: TaskItemDto): void {
    const isRestore = task.isDeleted;
    const confirmQuestion = isRestore
      ? `Restore task ${task.title} ?`
      : `Delete task ${task.title} ?`;
    this.snackbarService.confirm(confirmQuestion).subscribe((confirmed) => {
      if (confirmed) {
        isRestore ? this.restoreTask(task.id) : this.softDeleteTask(task.id);
      }
    });
  }

  private restoreTask(taskId: string): void {
    //alert('restore ' + taskId);
    this.loadingService.show();
    const subscription = this.taskApi.restoreTask(taskId).subscribe({
      next: () => {
        this.fetchAllUserTasks();
        this.snackbarService.success('Task Restored');
        this.loadingService.hide();
      },
      error: (err: any) => {
        console.error('Failed task restoration', err);
        this.snackbarService.error('Failed task restoration !');
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(subscription);
  }

  private softDeleteTask(taskId: string): void {
    //alert('soft delete ' + taskId);
    this.loadingService.show();
    const subscription = this.taskApi.softDeleteTask(taskId).subscribe({
      next: () => {
        this.snackbarService.success('Task Deleted');
        this.loadingService.hide();

        this.fetchAllUserTasks();
      },
      error: (err: any) => {
        console.error('Failed task deactivation', err);
        this.snackbarService.error('Failed task deactivation !');
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(subscription);
  }

  private fetchAllUserTasks(): void {
    const sub = this.taskApi.getTasksByUserId(this.userId, true).subscribe({
      next: (value: TaskItemDto[]) => {
        this.tasksOfUser = value;
        console.log(this.tasksOfUser);
        this.loadingService.hide();
      },
      error: (err: any) => {
        console.error('Failed to fetch user tasks', err);
        this.snackbarService.error(
          'Failed to fetch all user tasks for the logged in user !'
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }
}
