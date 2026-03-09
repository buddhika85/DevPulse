import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { CreateTaskDto } from '../../../../core/models/create-task.dto';
import { TaskFormDto } from '../../../../core/models/task-form.dto';
import { TaskItemDto } from '../../../../core/models/task-item.dto';
import { TaskPriority } from '../../../../core/models/task-priority.enum';
import { TaskStatus } from '../../../../core/models/task-status.enum';
import { UpdateTaskDto } from '../../../../core/models/update-task.dto';
import { LoadingService } from '../../../../core/services/loading-service';
import { TaskApiService } from '../../../../core/services/task-api';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { UserApiService } from '../../../../core/services/user-api';
import { UserAccountDto } from '../../../../core/models/user-account.dto';

@Component({
  selector: 'app-task-add-edit',
  imports: [ReactiveFormsModule],
  templateUrl: './task-add-edit.html',
  styleUrl: './task-add-edit.scss',
})
export class TaskAddEdit {
  private readonly activatedRoute: ActivatedRoute = inject(ActivatedRoute);
  private readonly userApiService: UserApiService = inject(UserApiService);
  private readonly taskApiService: TaskApiService = inject(TaskApiService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly loadingService: LoadingService = inject(LoadingService);
  private readonly snackbarService: SnackbarService = inject(SnackbarService);
  private readonly fb: FormBuilder = inject(FormBuilder);
  private readonly compositeSubscription: Subscription = new Subscription();

  private userId!: string;
  private isAddMode: boolean = true;
  private editId: string | null = null;

  readonly router: Router = inject(Router);

  testMessage: string = '';
  mainHeading!: string;
  originalTaskToEdit: TaskItemDto | null = null;
  taskFormDto: TaskFormDto = {
    id: null,
    userId: null,
    title: '',
    description: '',
    dueDate: new Date(),
    priority: TaskPriority.Low,
    status: TaskStatus.NotStarted,
  };
  teamMembers: UserAccountDto[] | null = null;

  // ddl - task status
  taskStatuses = Object.values(TaskStatus).map((status) => ({
    value: status,
    label: status,
  }));

  // ddl - task priority
  taskPriorities = Object.values(TaskPriority).map((priority) => ({
    value: priority,
    label: priority,
  }));

  // form group
  taskFormGroup!: FormGroup<{
    id: FormControl<string | null>;
    title: FormControl<string | null>;
    description: FormControl<string | null>;
    status: FormControl<string | null>;
    priority: FormControl<string | null>;
    dueDate: FormControl<string | null>;
    userId: FormControl<string | null>;
  }>;

  ngOnInit(): void {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    if (userId) {
      this.userId = userId;
      this.getTeamMembers();
      this.setupPage();
      return;
    }

    // no user Id means we cannot allow accessing this page
    this.router.navigate(['tasks']);
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  onSubmit(): void {
    if (this.taskFormGroup.valid) {
      // add mode
      if (this.isAddMode) {
        this.createNewTask();
      }

      // update mode
      else if (this.editId) {
        this.updateExistingTask();
      }
    }
  }

  private createNewTask(): void {
    const raw = this.taskFormGroup.getRawValue();
    const createTaskDto: CreateTaskDto = {
      userId: raw.userId ?? null,
      description: raw.description ?? '',
      title: raw.title ?? '',
      priority: raw.priority ?? 'Low',
      status: raw.status ?? 'NotStarted',
      dueDate: raw.dueDate ? new Date(raw.dueDate) : null,
    };

    console.log('Create: ', createTaskDto);

    this.loadingService.show();
    const sub = this.taskApiService.createTask(createTaskDto).subscribe({
      next: () => {
        this.snackbarService.success(
          `New task with title: ${createTaskDto.title} created successfuly!`,
        );
        this.router.navigate(['task-management']);
        this.loadingService.hide();
      },
      error: (err: any) => {
        this.snackbarService.error(
          `Error - New task with title: ${createTaskDto.title} creation failed !!`,
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }

  private updateExistingTask(): void {
    const raw = this.taskFormGroup.getRawValue();
    const updateTaskDto: UpdateTaskDto = {
      id: this.editId!,
      title: raw.title ?? '',
      description: raw.description ?? '',
      priority: raw.priority ?? 'Low',
      status: raw.status ?? 'Pending',
      dueDate: raw.dueDate ? new Date(raw.dueDate) : null,
    };

    console.log('Update: ', updateTaskDto);

    this.loadingService.show();
    const sub = this.taskApiService
      .updateTask(this.editId!, updateTaskDto)
      .subscribe({
        next: () => {
          this.snackbarService.success(
            `Task with title: ${updateTaskDto.title} updated successfuly!`,
          );
          this.router.navigate(['task-management']);
          this.loadingService.hide();
        },
        error: (err: any) => {
          this.snackbarService.error(
            `Error - Task with title: ${updateTaskDto.title} update failed !!`,
          );
          this.loadingService.hide();
        },
      });
    this.compositeSubscription.add(sub);
  }

  private getTeamMembers(): void {
    this.loadingService.show();
    const sub = this.userApiService.getTeamMembersForManager().subscribe({
      next: (value: UserAccountDto[]) => {
        this.teamMembers = value;
        this.loadingService.hide();
      },
      error: (err: any) => {
        console.error(
          'Error in fetching team members for manager',
          this.userId,
          err,
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }

  private setupPage(): void {
    this.editId = this.activatedRoute.snapshot.paramMap.get('id');
    this.isAddMode = this.editId === null;
    if (this.isAddMode) {
      this.setTaskFormDtoForInsert();
      this.mainHeading = 'Add New Task';
    } else {
      this.fetchTask();
      this.mainHeading = `Edit Task`;
    }
    this.testMessage = `Add: ${this.isAddMode} and edit id: ${this.editId}`;
  }

  private fetchTask() {
    if (this.editId) {
      this.loadingService.show();
      const sub = this.taskApiService.getTaskById(this.editId).subscribe({
        next: (value: TaskItemDto) => {
          this.originalTaskToEdit = value;
          this.mainHeading = `${this.mainHeading} - ${this.originalTaskToEdit.title}`;
          this.setTaskFormDtoForEdit();
          console.log('Edit : ', this.originalTaskToEdit);
          this.loadingService.hide();
        },
        error: (err: any) => {
          console.error(
            'Error in fetching task for edit with Id ',
            this.editId,
            err,
          );
          this.loadingService.hide();
        },
      });
      this.compositeSubscription.add(sub);
    }
  }

  private setTaskFormDtoForEdit(): void {
    this.taskFormDto = {
      id: this.originalTaskToEdit?.id ?? null,
      userId: this.originalTaskToEdit?.userId ?? null,
      title: this.originalTaskToEdit?.title ?? '',
      description: this.originalTaskToEdit?.description ?? '',
      dueDate: this.originalTaskToEdit?.dueDate
        ? new Date(this.originalTaskToEdit.dueDate)
        : new Date(),

      priority: this.originalTaskToEdit?.priority ?? TaskPriority.Low,
      status: this.originalTaskToEdit?.status ?? TaskStatus.NotStarted,
    };

    this.buildReactiveForm();
  }

  private setTaskFormDtoForInsert(): void {
    const dueDate = new Date();
    dueDate.setDate(dueDate.getDate() + 7); // 1 week after

    this.taskFormDto = {
      id: null,
      userId: null,
      title: '',
      description: '',
      dueDate: dueDate,
      priority: TaskPriority.Low,
      status: TaskStatus.NotStarted,
    };

    this.buildReactiveForm();
  }

  private buildReactiveForm(): void {
    this.taskFormGroup = this.fb.group({
      id: this.fb.control({ value: this.taskFormDto.id, disabled: true }),
      title: this.fb.control(
        { value: this.taskFormDto.title, disabled: false },
        {
          validators: [Validators.required, Validators.minLength(2)],
        },
      ),
      description: this.fb.control(this.taskFormDto.description, {
        validators: [Validators.required, Validators.minLength(2)],
      }),
      status: this.fb.control(this.taskFormDto.status, {
        validators: [Validators.required], // ddl
      }),
      priority: this.fb.control(this.taskFormDto.priority, {
        validators: [Validators.required], // ddl
      }),
      // Convert Date → "YYYY-MM-DD"
      dueDate: this.fb.control(
        {
          value: this.taskFormDto.dueDate
            ? this.taskFormDto.dueDate.toISOString().substring(0, 10)
            : null,
          disabled: false,
        },
        {
          validators: [Validators.required],
        },
      ),
      userId: this.fb.control(this.taskFormDto.userId, {
        validators: [Validators.required], // ddl
      }),
    });
  }
}
