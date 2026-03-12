import { Component, Inject } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogContent,
  MatDialogActions,
  MatDialogRef,
} from '@angular/material/dialog';
import { JournalEntryWithTasksAndFeedbackDto } from '../../../../core/models/journal-entry-with-tasks-and-feedback.dto';

@Component({
  selector: 'app-view-journal-dialog',
  imports: [MatDialogContent, MatDialogActions],
  templateUrl: './view-journal-dialog.html',
  styleUrl: './view-journal-dialog.scss',
})
export class ViewJournalDialog {
  constructor(
    private dialogRef: MatDialogRef<ViewJournalDialog>,
    @Inject(MAT_DIALOG_DATA)
    public journal: JournalEntryWithTasksAndFeedbackDto,
  ) {}

  onClose(): void {
    this.dialogRef.close(false);
  }
}
