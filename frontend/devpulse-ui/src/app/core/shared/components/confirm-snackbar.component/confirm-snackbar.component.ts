import { Component, Inject } from '@angular/core';
import {
  MAT_SNACK_BAR_DATA,
  MatSnackBarRef,
} from '@angular/material/snack-bar';

@Component({
  selector: 'app-confirm-snackbar.component',
  imports: [],
  templateUrl: './confirm-snackbar.component.html',
  styleUrl: './confirm-snackbar.component.scss',
})
export class ConfirmSnackbarComponent {
  constructor(
    public snackBarRef: MatSnackBarRef<ConfirmSnackbarComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public data: { confirmQuestion: string }
  ) {}

  onConfirm() {
    this.snackBarRef.dismissWithAction(); // signals "Yes"
  }

  onCancel() {
    this.snackBarRef.dismiss(); // signals "No"
  }
}
