import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskApiService } from '../../../../core/services/task-api';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { TaskItemDto } from '../../../../core/models/task-item.dto';
import { TaskStatus } from '../../../../core/models/task-status.enum';
import { TaskPriority } from '../../../../core/models/task-priority.enum';
import { TaskFormDto } from '../../../../core/models/task-form.dto';
import { CreateTaskDto } from '../../../../core/models/create-task.dto';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { UpdateTaskDto } from '../../../../core/models/update-task.dto';

@Component({
  selector: 'app-task-add-edit',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatRadioModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
  ],

  templateUrl: './task-add-edit.html',
  styleUrl: './task-add-edit.scss',
})
export class TaskAddEdit implements OnInit, OnDestroy {
  private readonly activatedRoute: ActivatedRoute = inject(ActivatedRoute);
  private readonly taskpiService: TaskApiService = inject(TaskApiService);
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
  taskFormDto!: TaskFormDto;

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
  }>;

  ngOnInit(): void {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    if (userId) {
      this.userId = userId;
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
      const raw = this.taskFormGroup.getRawValue();
      // add mode
      if (this.isAddMode) {
        const createTaskDto: CreateTaskDto = {
          userId: this.userId,
          description: raw.description ?? '',
          title: raw.title ?? '',
          priority: raw.priority ?? 'Low',
          status: raw.status ?? 'NotStarted',
          dueDate: raw.dueDate ? new Date(raw.dueDate) : null,
        };

        console.log('Create: ', createTaskDto);
      }

      // update mode
      else if (this.editId) {
        const updateTaskDto: UpdateTaskDto = {
          id: this.editId,
          title: raw.title ?? '',
          description: raw.description ?? '',
          priority: raw.priority ?? 'Low',
          status: raw.status ?? 'Pending',
          dueDate: raw.dueDate ? new Date(raw.dueDate) : null,
        };

        console.log('Update: ', updateTaskDto);
      }
    }
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
      const sub = this.taskpiService.getTaskById(this.editId).subscribe({
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
            err
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
      title: this.fb.control(this.taskFormDto.title, {
        validators: [Validators.required, Validators.minLength(2)],
      }),
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
        this.taskFormDto.dueDate
          ? this.taskFormDto.dueDate.toISOString().substring(0, 10)
          : null,
        {
          validators: [Validators.required],
        }
      ),
    });
  }
}
