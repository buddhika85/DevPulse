import { Component, Inject } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
} from '@angular/material/dialog';
import { TeamJournalEntryWithTasksAndFeedbackDto } from '../../../../core/models/journal-entry-with-tasks-and-feedback.dto';

@Component({
  selector: 'app-add-edit-journal-feedback-dialog',
  imports: [MatDialogActions, MatDialogContent],
  templateUrl: './add-edit-journal-feedback-dialog.html',
  styleUrl: './add-edit-journal-feedback-dialog.scss',
})
export class AddEditJournalFeedbackDialog {
  constructor(
    private dialogRef: MatDialogRef<AddEditJournalFeedbackDialog>,
    @Inject(MAT_DIALOG_DATA)
    public journal: TeamJournalEntryWithTasksAndFeedbackDto,
  ) {}

  onClose(): void {
    this.dialogRef.close(true);
  }
}
