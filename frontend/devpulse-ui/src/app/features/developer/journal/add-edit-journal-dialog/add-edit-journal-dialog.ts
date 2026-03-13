import { Component, inject, Inject, OnDestroy, OnInit } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
} from '@angular/material/dialog';
import {
  AddJournalEntryDto,
  CreateJournalDto,
  JournalEntryWithTasksAndFeedbackDto,
} from '../../../../core/models/journal-entry-with-tasks-and-feedback.dto';

import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { JournalFormDto } from '../../../../core/models/journal-form.dto';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { OrchestratorApiService } from '../../../../core/services/orchestrator-api';

@Component({
  selector: 'app-add-edit-journal-dialog',
  imports: [MatDialogActions, MatDialogContent, ReactiveFormsModule],
  templateUrl: './add-edit-journal-dialog.html',
  styleUrl: './add-edit-journal-dialog.scss',
})
export class AddEditJournalDialog implements OnInit, OnDestroy {
  private readonly loadingService: LoadingService = inject(LoadingService);
  private readonly snackbarService: SnackbarService = inject(SnackbarService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly orchestratorApi = inject(OrchestratorApiService);
  private readonly fb: FormBuilder = inject(FormBuilder);
  private readonly compositeSubscription: Subscription = new Subscription();

  private userId!: string;
  isAddMode: boolean = true;
  private addEditSuccess: boolean = false;
  mainHeading: string = 'Write New Journal';
  journalFormDto!: JournalFormDto;

  // ddl - tasks list - now this data is coming from Journal list page.
  // this to fix lag when using tasks ddl for the first time in dialog.
  // private tasksOfUser: TaskItemDto[] = [];
  tasksToSelect: DropDownListItem[] = [];

  journalFormGroup!: FormGroup<{
    id: FormControl<string | null>;
    title: FormControl<string | null>;
    content: FormControl<string | null>;
    linkedTasks: FormControl<string[] | null>;
  }>;

  constructor(
    private dialogRef: MatDialogRef<AddEditJournalDialog>,
    @Inject(MAT_DIALOG_DATA)
    private data: {
      journalToEdit: JournalEntryWithTasksAndFeedbackDto | null;
      tasksToSelect: DropDownListItem[];
    },
  ) {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    if (userId) {
      this.tasksToSelect = data.tasksToSelect;
      this.userId = userId;
      this.setupPage();
      return;
    }
  }

  ngOnInit(): void {
    if (this.data.journalToEdit) {
      this.isAddMode = false; // Edit Mode
    }

    this.setupPage();
  }

  private setupPage(): void {
    if (this.isAddMode) {
      this.setJournalFormForInsert();
      return;
    }
    // To Do: Phase 2
    this.mainHeading = `Edit Journal: ${this.data.journalToEdit!.title} with ID: ${this.data.journalToEdit!.idSnippet}`;
    this.setJournalFormForUpdate();
  }

  private setJournalFormForInsert(): void {
    this.journalFormDto = {
      id: null,
      userId: null,
      title: '',
      content: '',
      linkedTasks: [],
    };

    this.buildReactiveForm();
  }

  private buildReactiveForm(): void {
    /*
      journalFormGroup!: FormGroup<{
    id: FormControl<string | null>;
    title: FormControl<string | null>;
    content: FormControl<string | null>;
    linkedTasks: FormControl<string[] | null>;
  }>;
    
    */
    this.journalFormGroup = this.fb.group({
      id: this.fb.control({ value: this.journalFormDto.id, disabled: true }),
      title: this.fb.control(
        { value: this.journalFormDto.title, disabled: false },
        {
          validators: [Validators.required, Validators.minLength(5)],
        },
      ),
      content: this.fb.control(this.journalFormDto.content, {
        validators: [Validators.required, Validators.minLength(10)],
      }),
      linkedTasks: this.fb.control(this.journalFormDto.linkedTasks, {
        validators: [Validators.required], // ddl
      }),
    });
  }

  private setJournalFormForUpdate(): void {
    // To Do: Phase 2
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  onSubmit(): void {
    if (this.journalFormGroup.valid) {
      // add mode
      if (this.isAddMode) {
        this.createNewJournal();
      }

      // update mode
      else if (this.data.journalToEdit) {
        //this.updateExistingJournal();
      }
    }
  }

  onClose(): void {
    this.dialogRef.close(this.addEditSuccess);
  }

  private createNewJournal() {
    if (this.journalFormGroup.invalid) return;

    const raw = this.journalFormGroup.getRawValue();
    const addJournalEntryDto: AddJournalEntryDto = {
      userId: this.userId,
      title: raw.title ?? '',
      content: raw.content ?? '',
    };
    const createJournalDto: CreateJournalDto = {
      linkedTaskIds: raw.linkedTasks ?? [],
      addJournalEntryDto: addJournalEntryDto,
    };

    console.log('Create Journal: ', createJournalDto);

    this.loadingService.show();
    const sub = this.orchestratorApi
      .addJournalWithTaskLinks(createJournalDto)
      .subscribe({
        next: () => {
          this.snackbarService.success(
            `New journal with title: ${createJournalDto.addJournalEntryDto.title} created successfuly!`,
          );
          this.addEditSuccess = true;
          this.loadingService.hide();

          this.onClose();
        },
        error: (err: any) => {
          this.snackbarService.error(
            `Error - New journal with title: ${createJournalDto.addJournalEntryDto.title} creation failed !!`,
          );
          this.loadingService.hide();
        },
      });
    this.compositeSubscription.add(sub);
  }

  // private fetchMyTasks() {
  //   this.loadingService.show();
  //   const sub = this.taskApiService.getMyTasks(true).subscribe({
  //     next: (value: TaskItemDto[]) => {
  //       this.tasksOfUser = value;
  //       //console.log(this.tasksOfUser);

  //       this.tasksToSelect = this.tasksOfUser.map((task) => ({
  //         value: task.id,
  //         label: task.title,
  //       }));

  //       this.loadingService.hide();
  //     },
  //     error: (err: any) => {
  //       console.error('Failed to fetch tasks list Of User', err);
  //       this.snackbarService.error(
  //         'Failed to fetch all user tasks for the logged in user !',
  //       );
  //       this.loadingService.hide();
  //     },
  //   });
  //   this.compositeSubscription.add(sub);
  // }
}
