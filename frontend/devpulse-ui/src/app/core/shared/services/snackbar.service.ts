import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  constructor(private snackBar: MatSnackBar) {}

  success(message: string, action: string = 'Close') {
    this.snackBar.open(message, action, {
      duration: 300000,
      panelClass: ['snackbar-success'],
      horizontalPosition: 'right',
      verticalPosition: 'top',
    });
  }

  error(message: string, action: string = 'Dismiss') {
    this.snackBar.open(message, action, {
      duration: 5000,
      panelClass: ['snackbar-error'],
      horizontalPosition: 'right',
      verticalPosition: 'top',
    });
  }

  info(message: string, action: string = 'OK') {
    this.snackBar.open(message, action, {
      duration: 4000,
      panelClass: ['snackbar-info'],
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });
  }
}
