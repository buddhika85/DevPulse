import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TaskApiService } from '../../../../core/services/task-api';
import { Subscription } from 'rxjs';
import { TaskItemDto } from '../../../../core/models/task-item.dto';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { LoadingService } from '../../../../core/services/loading-service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';

@Component({
  selector: 'app-task-list',
  imports: [],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss',
})
export class TaskList implements OnInit {
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
      this.fetchAllUserTasks(userId);
    }
  }

  add(): void {
    this.router.navigate(['tasks/add']);
  }

  edit(id: number): void {
    this.router.navigate(['tasks/edit', id]);
  }

  private fetchAllUserTasks(userId: string): void {
    const sub = this.taskApi.getTasksByUserId(userId).subscribe({
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
