import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import {
  AddJournalEntryDto,
  CreateJournalDto,
  JournalEntryWithTasksAndFeedbackDto,
} from '../../../../core/models/journal-entry-with-tasks-and-feedback.dto';
import { LoadingService } from '../../../../core/services/loading-service';
import { OrchestratorApiService } from '../../../../core/services/orchestrator-api';
import { UserStoreService } from '../../../../core/services/user-store.service';
import { SnackbarService } from '../../../../core/shared/services/snackbar.service';
import { TableColumn } from '../../../../core/models/table-column';
import { TableAction } from '../../../../core/models/table-action';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';

@Component({
  selector: 'app-journal-list',
  imports: [GenericTableComponent],
  templateUrl: './journal-list.html',
  styleUrl: './journal-list.scss',
})
export class JournalList implements OnInit, OnDestroy {
  readonly columns: TableColumn[] = [
    { key: 'idSnippet', label: 'ID' },
    { key: 'title', label: 'Title' },
    { key: 'contentSnippet', label: 'Content snip' },
    { key: 'createdAtStr', label: 'Created' },
    { key: 'linkedTaskTitlesCsv', label: 'Tasks Linked' },
    { key: 'isFeedbackGivenStr', label: 'Feedback Given' },

    { key: 'isFeedbackSeenByUser', label: 'Feedback Viewed' },
    { key: 'feedbackManager', label: 'Feedback Manager' },
    { key: 'isDeletedStr', label: 'Is Deleted?' },
  ];

  readonly actions: TableAction[] = [
    {
      label: '',
      color: '#e4e2e2',
      icon: 'summarize',
      action: 'viewContent',
      tooltip: 'View Content',
    },
    {
      label: '',
      color: '#e4e2e2',
      icon: 'receipt',
      action: 'viewFeedback',
      tooltip: 'View Manager Feedback',
    },
    {
      label: '',
      color: '#bdb6b6',
      icon: 'transform',
      action: 'activateOrDeactivate',
      tooltip: 'activate / deactivate',
    },
  ];

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

  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
  }

  handleAction(event: {
    action: string;
    row: JournalEntryWithTasksAndFeedbackDto;
  }) {
    if (event.action === 'viewFeedback') {
      this.viewFeedback(event.row);
    } else if (event.action === 'viewContent') {
      this.viewContent(event.row);
    } else if (event.action === 'activateOrDeactivate') {
      this.restoreOrSoftDelete(event.row);
    }
  }

  writeJournal(): void {
    // To Do: open popup

    // example
    const testJournalNum = this.journalsOfUser.length + 1;
    const addJournalEntryDto: AddJournalEntryDto = {
      userId: this.userId,
      title: `Journal: ${testJournalNum} created from code`,
      content: `Content --- Journal: ${testJournalNum}`,
    };
    const createJournalDto: CreateJournalDto = {
      linkedTaskIds: [
        // '6f130967-0166-47f1-c809-08de112c99d4', // Design dashboard
        // '04691f48-b1a4-4585-c80a-08de112c99d4', // Write API tests

        'c96331ab-d853-4e65-df35-08de1ce25178', // Test Linked Task 1
        'eb86cc38-bad1-4653-df36-08de1ce25178', // Test Linked Task 2
      ],
      addJournalEntryDto: addJournalEntryDto,
    };

    // add API call
    console.log(`Test Journal To Create: ${testJournalNum}`, createJournalDto);
    this.loadingService.show();
    const sub = this.orchestratorApi
      .addJournalWithTaskLinks(createJournalDto)
      .subscribe({
        next: () => {
          this.snackbarService.success(
            `New journal with title: ${createJournalDto.addJournalEntryDto.title} created successfuly!`,
          );
          this.fetchAllUserJournals();
          this.loadingService.hide();
        },
        error: (err: any) => {
          this.snackbarService.error(
            `Error - New journal with title: ${createJournalDto.addJournalEntryDto.title} creation failed !!`,
          );
          this.loadingService.hide();
        },
      });
    this.compositeSubscription.add(sub);
  }

  private viewFeedback(journal: JournalEntryWithTasksAndFeedbackDto): void {}

  private viewContent(journal: JournalEntryWithTasksAndFeedbackDto): void {}

  private restoreOrSoftDelete(
    journal: JournalEntryWithTasksAndFeedbackDto,
  ): void {
    // const isRestore = task.isDeleted;
    // const confirmQuestion = isRestore
    //   ? `Restore task ${task.title} ?`
    //   : `Delete task ${task.title} ?`;
    // this.snackbarService.confirm(confirmQuestion).subscribe((confirmed) => {
    //   if (confirmed) {
    //     isRestore ? this.restoreTask(task.id) : this.softDeleteTask(task.id);
    //   }
    // });
  }

  private restoreJournal(jorunalId: string): void {
    //alert('restore ' + taskId);
    // this.loadingService.show();
    // const subscription = this.taskApi.restoreTask(taskId).subscribe({
    //   next: () => {
    //     this.fetchAllUserTasks();
    //     this.snackbarService.success('Task Restored');
    //     this.loadingService.hide();
    //   },
    //   error: (err: any) => {
    //     console.error('Failed task restoration', err);
    //     this.snackbarService.error('Failed task restoration !');
    //     this.loadingService.hide();
    //   },
    // });
    // this.compositeSubscription.add(subscription);
  }

  private softDeleteJournal(jorunalId: string): void {
    // //alert('soft delete ' + taskId);
    // this.loadingService.show();
    // const subscription = this.taskApi.softDeleteTask(taskId).subscribe({
    //   next: () => {
    //     this.snackbarService.success('Task Deleted');
    //     this.loadingService.hide();
    //     this.fetchAllUserTasks();
    //   },
    //   error: (err: any) => {
    //     console.error('Failed task deactivation', err);
    //     this.snackbarService.error('Failed task deactivation !');
    //     this.loadingService.hide();
    //   },
    // });
    // this.compositeSubscription.add(subscription);
  }

  private fetchAllUserJournals() {
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
