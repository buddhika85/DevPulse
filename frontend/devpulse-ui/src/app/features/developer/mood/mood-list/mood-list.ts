import { Component, inject, OnInit } from '@angular/core';
import { MoodApiService } from '../../../../core/services/mood-api';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { MoodEntryDto } from '../../../../core/models/mood-entry.dto';
import { TableColumn } from '../../../../core/models/table-column';
import { TableAction } from '../../../../core/models/table-action';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';

@Component({
  selector: 'app-mood-list',
  imports: [GenericTableComponent],
  templateUrl: './mood-list.html',
  styleUrl: './mood-list.scss',
})
export class MoodList implements OnInit {
  readonly columns: TableColumn[] = [
    // { key: 'id', label: 'ID' },
    // { key: 'userId', label: 'User' },

    { key: 'dayStr', label: 'Date' },

    { key: 'moodTime', label: 'Session' },
    { key: 'moodTimeRange', label: 'Time' },

    { key: 'moodLevel', label: 'Mood Level' },
    { key: 'moodScore', label: 'Mood Score' },

    { key: 'note', label: 'Note' },
  ];

  readonly actions: TableAction[] = [
    {
      label: '',
      color: '#e4e2e2',
      icon: 'edit',
      action: 'edit',
      tooltip: 'edit',
    },
    {
      label: '',
      color: '#bdb6b6',
      icon: 'delete',
      action: 'delete',
      tooltip: 'remove',
    },
  ];

  readonly pageSizeOptions: number[] = [10, 20, 30, 40, 50, 100];

  private userId!: string;

  private readonly router: Router = inject(Router);
  private readonly moodApi = inject(MoodApiService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly loadingService = inject(LoadingService);
  private readonly compositeSubscription: Subscription = new Subscription();
  private readonly snackbarService: SnackbarService = inject(SnackbarService);

  moodsOfUser: MoodEntryDto[] = [];

  ngOnInit(): void {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    this.loadingService.show();
    if (userId) {
      this.userId = userId;
      this.fetchAllUserTasks();
    }
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  handleAction(event: { action: string; row: MoodEntryDto }) {
    if (event.action === 'edit') {
      this.edit(event.row);
    } else if (event.action === 'delete') {
      this.delete(event.row);
    }
  }

  add(): void {
    alert('add');
    this.router.navigate(['moods/add']);
  }

  edit(mood: MoodEntryDto): void {
    alert('edit: ' + JSON.stringify(mood));
    this.router.navigate(['moods/edit', mood.id]);
  }

  private delete(mood: MoodEntryDto): void {
    this.snackbarService
      .confirm(
        `Delete mood for : ${mood.dayStr} for session : ${mood.moodTime} with mood level: ${mood.moodLevel} ?`
      )
      .subscribe((confirmed) => {
        if (confirmed) {
          this.deleteMood(mood.id);
        }
      });
  }

  private deleteMood(id: string): void {
    //this.moodApi.delete()
  }

  private fetchAllUserTasks(): void {
    const sub = this.moodApi.getMoodsByUserId(this.userId).subscribe({
      next: (value: MoodEntryDto[]) => {
        this.moodsOfUser = value;
        console.log(this.moodsOfUser);
        this.loadingService.hide();
      },
      error: (err: any) => {
        console.error('Failed to fetch user moods list', err);
        this.snackbarService.error(
          'Failed to fetch all user moods for the logged in user !'
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }
}
