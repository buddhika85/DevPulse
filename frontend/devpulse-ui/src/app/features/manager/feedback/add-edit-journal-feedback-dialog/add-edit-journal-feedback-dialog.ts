import { Component, inject, Inject, OnDestroy } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
} from '@angular/material/dialog';
import { TeamJournalEntryWithTasksAndFeedbackDto } from '../../../../core/models/journal-entry-with-tasks-and-feedback.dto';
import {
  FormGroup,
  FormControl,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { CommonModule } from '@angular/common';
import { AddJournalFeedbackDto } from '../../../../core/models/add-journal-feedback.dto';
import { JournalApiService } from '../../../../core/services/journal-api';

@Component({
  selector: 'app-add-edit-journal-feedback-dialog',
  imports: [
    MatDialogActions,
    MatDialogContent,
    ReactiveFormsModule,
    CommonModule,
  ],
  templateUrl: './add-edit-journal-feedback-dialog.html',
  styleUrl: './add-edit-journal-feedback-dialog.scss',
})
export class AddEditJournalFeedbackDialog implements OnDestroy {
  private readonly fb: FormBuilder = inject(FormBuilder);
  private readonly compositeSubscription: Subscription = new Subscription();
  private readonly loadingService: LoadingService = inject(LoadingService);
  private readonly snackbarService: SnackbarService = inject(SnackbarService);
  private readonly journalApiService: JournalApiService =
    inject(JournalApiService);

  private feedbackSaved: boolean = false;

  feedbackFormGroup!: FormGroup<{
    content: FormControl<string | null>;
  }>;

  public readonly journal!: TeamJournalEntryWithTasksAndFeedbackDto;
  public readonly managerId!: string;

  constructor(
    private dialogRef: MatDialogRef<AddEditJournalFeedbackDialog>,
    @Inject(MAT_DIALOG_DATA)
    private data: {
      journal: TeamJournalEntryWithTasksAndFeedbackDto;
      managerId: string;
    },
  ) {
    this.journal = data.journal;
    this.managerId = data.managerId;
    this.buildReactiveForm();
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  onClose(): void {
    this.dialogRef.close(this.feedbackSaved);
  }

  onSubmit(): void {
    if (this.feedbackFormGroup.valid) {
      const addJournalFeedbackDto: AddJournalFeedbackDto = {
        jounralEntryId: this.journal.id,
        comment: this.feedbackFormGroup.getRawValue().content ?? '',
        feedbackManagerId: this.managerId,
      };

      this.loadingService.show();
      const sub = this.journalApiService
        .addJournalFeedback(addJournalFeedbackDto)
        .subscribe({
          next: () => {
            this.snackbarService.success(
              `Feedback added to journal with title: ${this.journal.title} successfuly!`,
            );
            this.feedbackSaved = true;
            this.loadingService.hide();

            this.onClose();
          },
          error: (err: any) => {
            this.snackbarService.error(
              `Error - Feedback creation for journal with title: ${this.journal.title} failed !!`,
            );
            this.loadingService.hide();
          },
        });
      this.compositeSubscription.add(sub);
    }
  }

  private buildReactiveForm(): void {
    this.feedbackFormGroup = this.fb.group({
      content: this.fb.control('', {
        validators: [Validators.required, Validators.minLength(10)],
      }),
    });
  }
}
