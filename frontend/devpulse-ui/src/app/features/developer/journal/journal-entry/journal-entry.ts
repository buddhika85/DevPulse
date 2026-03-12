import { Component, inject } from '@angular/core';
import { JournalEntryWithTasksAndFeedbackDto } from '../../../../core/models/journal-entry-with-tasks-and-feedback.dto';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../../core/services/loading-service';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { OrchestratorApiService } from '../../../../core/services/orchestrator-api';

@Component({
  selector: 'app-journal-entry',
  imports: [],
  templateUrl: './journal-entry.html',
  styleUrl: './journal-entry.scss',
})
export class JournalEntry {
  readonly pageSizeOptions: number[] = [10, 20, 30, 40, 50, 100];

  private userId!: string;

  private readonly router: Router = inject(Router);
  private readonly orchestratorApi = inject(OrchestratorApiService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly loadingService = inject(LoadingService);
  private readonly compositeSubscription: Subscription = new Subscription();
  private readonly snackbarService: SnackbarService = inject(SnackbarService);

  journalsOfUser: JournalEntryWithTasksAndFeedbackDto[] = [];

  ngOnInit(): void {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    this.loadingService.show();
    if (userId) {
      this.userId = userId;
      this.fetchAllUserJournals();
    }
  }
  fetchAllUserJournals() {
    const sub = this.orchestratorApi.getMyJournals(true).subscribe({
      next: (value: JournalEntryWithTasksAndFeedbackDto[]) => {
        this.journalsOfUser = value;
        console.log(this.journalsOfUser);
        this.loadingService.hide();
      },
      error: (err: any) => {
        console.error('Failed to fetch journals Of User', err);
        this.snackbarService.error(
          'Failed to fetch all user journals for the logged in user !',
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }
}
