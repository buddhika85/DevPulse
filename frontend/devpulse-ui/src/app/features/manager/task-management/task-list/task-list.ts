import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { TableAction } from '../../../../core/models/table-action';
import { TableColumn } from '../../../../core/models/table-column';
import {
  TaskItemDto,
  TaskItemWithUserDto,
} from '../../../../core/models/task-item.dto';
import { LoadingService } from '../../../../core/services/loading-service';
import { TaskApiService } from '../../../../core/services/task-api';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';
import { OrchestratorApiService } from '../../../../core/services/orchestrator-api';
import { GridButtonStyle } from '../../../../shared/enums/grid-button-style.enum';

@Component({
  selector: 'app-task-list',
  imports: [GenericTableComponent],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss',
})
export class TaskList {
  readonly columns: TableColumn[] = [
    // { key: 'id', label: 'ID' },
    { key: 'title', label: 'Title' },
    { key: 'description', label: 'Description' },
    { key: 'status', label: 'Status' },
    { key: 'createdAtStr', label: 'Created' },
    { key: 'dueDateStr', label: 'Due' },

    { key: 'priority', label: 'Priority' },
    { key: 'isDeletedStr', label: 'Is Deleted?' },

    //{ key: 'userId', label: 'UserId' },

    { key: 'userDisplayName', label: 'User' },
  ];

  readonly actions: TableAction[] = [
    // {
    //   label: '',
    //   color: '#e4e2e2',
    //   icon: 'edit',
    //   action: 'edit',
    //   tooltip: 'edit',
    // },
    {
      label: 'De / Activate',
      // color: '#bdb6b6',
      class: GridButtonStyle.AzureGridDangerBtn,
      icon: 'transform',
      action: 'activateOrDeactivate',
      tooltip: 'activate / deactivate',
    },
  ];

  readonly pageSizeOptions: number[] = [10, 20, 30, 40, 50, 100];

  private userId!: string;

  private readonly router: Router = inject(Router);
  private readonly taskApi = inject(TaskApiService);
  private readonly orchestratorApi = inject(OrchestratorApiService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly loadingService = inject(LoadingService);
  private readonly compositeSubscription: Subscription = new Subscription();
  private readonly snackbarService: SnackbarService = inject(SnackbarService);

  tasksOfManager: TaskItemWithUserDto[] = [];

  ngOnInit(): void {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    this.loadingService.show();
    if (userId) {
      this.userId = userId;
      this.fetchAllManagedTasks();
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
    this.router.navigate(['task-management/add']);
  }

  edit(task: TaskItemDto): void {
    this.router.navigate(['task-management/edit', task.id]);
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
        this.fetchAllManagedTasks();
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

        this.fetchAllManagedTasks();
      },
      error: (err: any) => {
        console.error('Failed task deactivation', err);
        this.snackbarService.error('Failed task deactivation !');
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(subscription);
  }

  private fetchAllManagedTasks(): void {
    const sub = this.orchestratorApi.getManagedTasks(true).subscribe({
      next: (value: TaskItemWithUserDto[]) => {
        this.tasksOfManager = value;
        console.log(this.tasksOfManager);
        this.loadingService.hide();
      },
      error: (err: any) => {
        console.error('Failed to fetch managed tasks', err);
        this.snackbarService.error(
          'Failed to fetch all managed tasks for the logged in manager !',
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }
}
