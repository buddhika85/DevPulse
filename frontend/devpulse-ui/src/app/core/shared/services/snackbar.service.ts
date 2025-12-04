import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmSnackbarComponent } from '../components/confirm-snackbar.component/confirm-snackbar.component';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  constructor(private snackBar: MatSnackBar) {}

  success(message: string, action: string = 'Close') {
    this.snackBar.open(message, action, {
      duration: 5000,
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

  // 'Yes' or 'No' question
  confirm(confirmQuestion: string): Observable<boolean> {
    const ref = this.snackBar.openFromComponent(ConfirmSnackbarComponent, {
      data: { confirmQuestion },
      duration: 0, // keep open until user clicks
      panelClass: ['snackbar-confirm'],
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });

    return ref.afterDismissed().pipe(
      map((info) => info.dismissedByAction) // true = Yes, false = No
    );
  }

  /* using promises */
  // confirm(confirmQuestion: string): Promise<boolean> {
  //   const ref = this.snackBar.openFromComponent(ConfirmSnackbarComponent, {
  //     data: { confirmQuestion },
  //     duration: 0,
  //     panelClass: ['snackbar-confirm'],
  //   });

  //   return new Promise((resolve) => {
  //     ref.afterDismissed().subscribe((info) => {
  //       resolve(info.dismissedByAction); // true = Yes, false = No
  //     });
  //   });
  // }

  /* calling the above */
  //   const confirmed = await this.confirm('Delete user 123?');
  // if (confirmed) {
  //   // YES clicked
  // } else {
  //   // NO clicked
  // }
}
