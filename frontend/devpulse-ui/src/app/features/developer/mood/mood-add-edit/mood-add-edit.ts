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
import { MoodApiService } from '../../../../core/services/mood-api';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { MoodTime } from '../../../../core/models/mood-time.enum';
import { MoodLevel } from '../../../../core/models/mood-level.enum';
import { MoodEntryDto } from '../../../../core/models/mood-entry.dto';
import { MoodFormDto } from '../../../../core/models/mood-form.dto';
import { AddMoodEntryDto } from '../../../../core/models/add-mood-entry.dto';
import { UpdateMoodEntryDto } from '../../../../core/models/update-mood-entry.dto';

@Component({
  selector: 'app-mood-add-edit',
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
  templateUrl: './mood-add-edit.html',
  styleUrl: './mood-add-edit.scss',
})
export class MoodAddEdit implements OnInit, OnDestroy {
  private readonly activatedRoute: ActivatedRoute = inject(ActivatedRoute);
  private readonly moodApiService: MoodApiService = inject(MoodApiService);
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
  originalMoodToEdit: MoodEntryDto | null = null;
  moodFormDto!: MoodFormDto;

  // ddl - mood time
  moodTimes = Object.keys(MoodTime).map((key) => ({
    value: key,
    label: MoodTime[key as keyof typeof MoodTime],
  }));
  // ddl - mood level
  moodLevels = Object.keys(MoodLevel).map((key) => ({
    value: key,
    label: MoodLevel[key as keyof typeof MoodLevel],
  }));

  // form group
  moodFormGroup!: FormGroup<{
    id: FormControl<string | null>;
    day: FormControl<string | null>;
    moodTime: FormControl<string | null>;
    moodLevel: FormControl<string | null>;
    note: FormControl<string | null>;
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
    this.router.navigate(['moods']);
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }
  onSubmit(): void {
    if (this.moodFormGroup.valid) {
      // add mode
      if (this.isAddMode) {
        this.addMoodEntryFlow();
      }
      // update mode
      else if (this.editId) {
        this.updateMoodEntryFlow();
      }
    }
  }

  private addMoodEntryFlow(): void {
    const raw = this.moodFormGroup.getRawValue();

    this.loadingService.show();
    const sub = this.moodApiService
      .isMoodEntryExists(this.userId, raw.day!, raw.moodTime!)
      .subscribe({
        next: (isExists: boolean) => {
          if (isExists) {
            this.snackbarService.info(
              `A mood already exists for ${raw.day} ${raw.moodTime}. So creation aborted !!`
            );
            this.loadingService.hide();
            return;
          }

          // no mood extry exists for selected day & time, so continue adding
          this.loadingService.hide();
          this.addNewMoodEntry(raw);
        },
        error: (err: any) => {
          this.snackbarService.error(
            `Error - New mood for ${raw.day} ${raw.moodTime} creation failed !!`
          );
          this.loadingService.hide();
        },
      });
    this.compositeSubscription.add(sub);
  }

  private addNewMoodEntry(raw: {
    id: string | null;
    day: string | null;
    moodTime: string | null;
    moodLevel: string | null;
    note: string | null;
  }): void {
    const addMoodEntryDto: AddMoodEntryDto = {
      userId: this.userId,
      day: raw.day,
      moodTime: raw.moodTime,
      moodLevel: raw.moodLevel,
      note: raw.note ?? '',
    };
    //console.log('Create: ', addMoodEntryDto);
    this.loadingService.show();
    const sub = this.moodApiService.createMood(addMoodEntryDto).subscribe({
      next: () => {
        this.snackbarService.success(
          `New mood for ${addMoodEntryDto.day} ${addMoodEntryDto.moodTime} created successfuly!`
        );
        this.router.navigate(['moods']);
        this.loadingService.hide();
      },
      error: (err: any) => {
        this.snackbarService.error(
          `Error - New mood for ${addMoodEntryDto.day} ${addMoodEntryDto.moodTime} creation failed !!`
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }

  private updateMoodEntryFlow(): void {
    const raw = this.moodFormGroup.getRawValue();

    this.loadingService.show();
    const sub = this.moodApiService
      .findOtherMoodEntry(
        this.originalMoodToEdit!.id,
        this.userId,
        raw.day!,
        raw.moodTime!
      )
      .subscribe({
        next: (isExists: boolean) => {
          if (isExists) {
            this.snackbarService.info(
              `Another mood already exists for ${raw.day} ${raw.moodTime}. So updated aborted !!`
            );
            this.loadingService.hide();
            return;
          }

          // no mood extry exists for selected day & time, so continue update
          this.loadingService.hide();
          this.updateMoodEntry(raw);
        },
        error: (err: any) => {
          this.snackbarService.error(
            `Error - Updating mood for ${raw.day} ${raw.moodTime} failed !!`
          );
          this.loadingService.hide();
        },
      });
    this.compositeSubscription.add(sub);
  }

  private updateMoodEntry(raw: {
    id: string | null;
    day: string | null;
    moodTime: string | null;
    moodLevel: string | null;
    note: string | null;
  }): void {
    const updateMoodEntryDto: UpdateMoodEntryDto = {
      id: this.originalMoodToEdit!.id,
      day: raw.day!,
      moodTime: raw.moodTime!,
      moodLevel: raw.moodLevel!,
      note: raw.note ?? '',
    };
    //console.log('Update: ', updateMoodEntryDto);
    this.loadingService.show();
    const sub = this.moodApiService
      .updateMood(this.originalMoodToEdit!.id, updateMoodEntryDto)
      .subscribe({
        next: () => {
          this.snackbarService.success(
            `Updating mood for ${updateMoodEntryDto.day} ${updateMoodEntryDto.moodTime} successful !!`
          );
          this.router.navigate(['moods']);
          this.loadingService.hide();
        },
        error: (err: any) => {
          this.snackbarService.error(
            `Error - Updating mood for ${updateMoodEntryDto.day} ${updateMoodEntryDto.moodTime} failed !!`
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
      this.setMoodFormDtoForInsert();
      this.mainHeading = 'Record a new Mood';
    } else {
      this.fetchMood();
      this.mainHeading = `Modify a Mood`;
    }
    this.testMessage = `Add: ${this.isAddMode} and edit id: ${this.editId}`;
  }

  private fetchMood() {
    if (this.editId) {
      this.loadingService.show();
      const sub = this.moodApiService.getMoodById(this.editId).subscribe({
        next: (value: MoodEntryDto) => {
          this.originalMoodToEdit = value;
          this.mainHeading = `${this.mainHeading} - 
                              ${this.originalMoodToEdit.dayStr}: 
                              ${this.originalMoodToEdit.moodTime} => 
                              ${this.originalMoodToEdit.moodLevel}`;
          this.setMoodFormDtoForEdit();
          console.log('Edit : ', this.originalMoodToEdit);
          this.loadingService.hide();
        },
        error: (err: any) => {
          console.error(
            'Error in fetching mood for edit with Id ',
            this.editId,
            err
          );
          this.loadingService.hide();
        },
      });
      this.compositeSubscription.add(sub);
    }
  }

  private setMoodFormDtoForEdit(): void {
    if (this.originalMoodToEdit) {
      this.moodFormDto = {
        id: this.originalMoodToEdit.id ?? null,
        day: new Date(this.originalMoodToEdit.dayStr),
        moodTime: this.originalMoodToEdit.moodTime,
        moodLevel: this.originalMoodToEdit.moodLevel,
        note: this.originalMoodToEdit.note,
      };
      this.buildReactiveForm();
    }
  }

  private setMoodFormDtoForInsert(): void {
    this.moodFormDto = {
      id: null,
      day: new Date(),
      moodTime: 'Morning',
      moodLevel: 'Neutral',
      note: null,
    };
    this.buildReactiveForm();
  }

  private buildReactiveForm(): void {
    this.moodFormGroup = this.fb.group({
      id: this.fb.control({ value: this.moodFormDto.id, disabled: true }),

      // Convert Date → "YYYY-MM-DD"
      day: this.fb.control(
        this.moodFormDto.day
          ? this.moodFormDto.day.toISOString().substring(0, 10)
          : null,
        {
          validators: [Validators.required],
        }
      ),

      moodTime: this.fb.control(this.moodFormDto.moodTime, {
        validators: [Validators.required], // ddl
      }),

      moodLevel: this.fb.control(this.moodFormDto.moodLevel, {
        validators: [Validators.required], // ddl
      }),

      note: this.fb.control(this.moodFormDto.note, {
        validators: [Validators.minLength(2)],
      }),
    });
  }
}
