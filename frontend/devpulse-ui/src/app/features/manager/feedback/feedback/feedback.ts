import { Component, inject } from '@angular/core';
import { TableColumn } from '../../../../core/models/table-column';
import { TableAction } from '../../../../core/models/table-action';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { TeamJournalEntryWithTasksAndFeedbackDto } from '../../../../core/models/journal-entry-with-tasks-and-feedback.dto';
import { LoadingService } from '../../../../core/services/loading-service';
import { OrchestratorApiService } from '../../../../core/services/orchestrator-api';
import { TaskApiService } from '../../../../core/services/task-api';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { ViewJournalDialog } from '../../../developer/journal/view-journal-dialog/view-journal-dialog';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';
import { GridButtonStyle } from '../../../../shared/enums/grid-button-style.enum';

@Component({
  selector: 'app-feedback',
  imports: [GenericTableComponent],
  templateUrl: './feedback.html',
  styleUrl: './feedback.scss',
})
export class Feedback {
  readonly columns: TableColumn[] = [
    { key: 'idSnippet', label: 'ID' },
    { key: 'title', label: 'Title' },
    { key: 'userDisplayName', label: 'Team Member' },
    { key: 'contentSnippet', label: 'Content snip' },
    { key: 'createdAtStr', label: 'Created' },
    { key: 'linkedTaskTitlesCsv', label: 'Tasks Linked' },
    { key: 'isFeedbackGivenStr', label: 'Feedback Given' },

    { key: 'isFeedbackSeenByUser', label: 'Feedback Viewed' },
    { key: 'feedbackManager', label: 'Feedback Manager' },
    // { key: 'isDeletedStr', label: 'Is Deleted?' },
  ];

  readonly actions: TableAction[] = [
    {
      label: 'Feedback',
      class: GridButtonStyle.AzureGridBtn,
      action: 'writeFeedback',
      tooltip: '',
    },
    {
      label: 'View',
      color: GridButtonStyle.AzureGridTextBtn,
      action: 'viewJournalWithFeedback',
      tooltip: '',
    },
  ];

  readonly pageSizeOptions: number[] = [10, 20, 30, 40, 50, 100];

  private userId!: string;

  private readonly router: Router = inject(Router);
  private readonly dialog: MatDialog = inject(MatDialog);
  private readonly orchestratorApi = inject(OrchestratorApiService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly taskApiService: TaskApiService = inject(TaskApiService);
  private readonly loadingService = inject(LoadingService);
  private readonly compositeSubscription: Subscription = new Subscription();
  private readonly snackbarService: SnackbarService = inject(SnackbarService);

  journalsOfTeam: TeamJournalEntryWithTasksAndFeedbackDto[] = [];

  ngOnInit(): void {
    const user = this.userStoreService.userDto();
    const userId = user?.id;
    this.loadingService.show();
    if (userId) {
      this.userId = userId;
      this.fetchAllTeamJournals();
    }
  }

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  handleAction(event: {
    action: string;
    row: TeamJournalEntryWithTasksAndFeedbackDto;
  }) {
    const journal = event.row;
    if (event.action === 'viewJournalWithFeedback') {
      this.viewJournalWithFeedback(journal);
    } else if (event.action === 'writeFeedback') {
      if (journal.isFeedbackGiven) {
        this.snackbarService.info(
          `Journal titled: ${journal.title} of ${journal.userDisplayName} already given with a feedback. !`,
        );
        return;
      }
      this.writeJournalFeedback(journal);
    }
  }

  private writeJournalFeedback(
    journal: TeamJournalEntryWithTasksAndFeedbackDto,
  ) {
    alert('To Do');
  }

  private viewJournalWithFeedback(
    journal: TeamJournalEntryWithTasksAndFeedbackDto,
  ): void {
    // alert('viewJournalWithFeedback - ' + journal.id);

    (document.activeElement as HTMLElement)?.blur();

    // ref - https://github.com/buddhika85/LibraryManagementSystem_2025/blob/main/client/src/app/features/book-list/book-list.component.ts
    const dialogRef = this.dialog
      .open(ViewJournalDialog, {
        width: '650px',
        maxHeight: '900px',
        panelClass: 'journal-dialog-container',
        data: journal,
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          // nothing to do here as it is just a read only dialog
        }
      });
  }

  private fetchAllTeamJournals() {
    const sub = this.orchestratorApi.getTeamJournals(false).subscribe({
      next: (value: TeamJournalEntryWithTasksAndFeedbackDto[]) => {
        this.journalsOfTeam = value;
        console.log(this.journalsOfTeam);
        this.loadingService.hide();
      },
      error: (err: any) => {
        console.error('Failed to fetch journals Of Team', err);
        this.snackbarService.error(
          'Failed to fetch team journals for the logged in manager !',
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }
}
