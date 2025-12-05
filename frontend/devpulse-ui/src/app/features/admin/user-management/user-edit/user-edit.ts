import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { UserApiService } from '../../../../core/services/user-api';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { UserAccountDto } from '../../../../core/models/user-account.dto';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { UserRole } from '../../../../core/models/user-role.enum';

@Component({
  selector: 'app-user-edit',
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
  templateUrl: './user-edit.html',
  styleUrls: ['./user-edit.scss'],
})
export class UserEdit implements OnInit, OnDestroy {
  private readonly activatedRoute: ActivatedRoute = inject(ActivatedRoute);
  private readonly userApiService: UserApiService = inject(UserApiService);
  private readonly loadingService: LoadingService = inject(LoadingService);
  private readonly snackbarService: SnackbarService = inject(SnackbarService);
  private readonly fb: FormBuilder = inject(FormBuilder);
  private readonly compositeSubscription: Subscription = new Subscription();

  readonly router: Router = inject(Router);

  userFormGroup!: FormGroup<{
    id: FormControl<string | null>;
    email: FormControl<string | null>;
    displayName: FormControl<string | null>;
    createdAt: FormControl<string | null>;
    userRole: FormControl<UserRole | null>;
    managerName: FormControl<string | null>;
    isActive: FormControl<boolean | null>;
  }>;
  mainHeading!: string;
  user!: UserAccountDto; // from API
  managers!: UserAccountDto[]; // from API
  roles = Object.values(UserRole).map((role) => ({
    value: role,
    label: role,
  }));

  ngOnInit(): void {
    const userId = this.activatedRoute.snapshot.paramMap.get('id');
    if (userId) {
      this.fetchUserAndManagers(userId);
    }
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  // need more work
  onSubmit(): void {
    if (this.userFormGroup.valid) {
      const updatedUser = {
        ...this.user,
        ...this.userFormGroup.getRawValue(), // includes disabled fields if needed
      };
      // this.userApiService.updateUser(updatedUser).subscribe(() => {
      //   console.log('User updated successfully');
      // });
    }
  }

  private fetchUserAndManagers(userId: string) {
    this.loadingService.show();
    const sub = forkJoin({
      user: this.userApiService.getUserById(userId),
      managers: this.userApiService.getUsersByRole(UserRole.Manager),
    }).subscribe({
      next: ({ user, managers }) => {
        this.user = user;
        this.managers = managers;

        this.setUpPage();
        this.buildReactiveForm();

        console.log('Edit user:', user);
        console.log('Managers:', managers);

        this.loadingService.hide();
      },
      error: (err) => {
        console.error('Failed to fetch user or managers', err);
        this.snackbarService.error('Failed to fetch user or managers!');
        this.loadingService.hide();
      },
    });

    this.compositeSubscription.add(sub);
  }

  private setUpPage(): void {
    this.mainHeading = `Edit user: ${this.user.displayName}`;
  }

  private buildReactiveForm(): void {
    this.userFormGroup = this.fb.group({
      id: [{ value: this.user.id, disabled: true }], // read-only
      email: [{ value: this.user.email, disabled: true }], // read-only
      displayName: [
        this.user.displayName,
        [Validators.required, Validators.minLength(2)],
      ],
      createdAt: [{ value: this.user.createdAt, disabled: true }], // read-only
      userRole: [this.user.userRole, Validators.required], // dropdown
      managerName: [this.user.managerId], // dropdown
      isActive: [this.user.isActive], // radio toggle
    });
  }
}
