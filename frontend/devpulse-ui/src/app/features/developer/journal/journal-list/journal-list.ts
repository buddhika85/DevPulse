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
import { MatDialog } from '@angular/material/dialog';
import { ViewJournalDialog } from '../view-journal-dialog/view-journal-dialog';
import { AddEditJournalDialog } from '../add-edit-journal-dialog/add-edit-journal-dialog';
import { TaskItemDto } from '../../../../core/models/task-item.dto';
import { TaskApiService } from '../../../../core/services/task-api';
import { GridButtonStyle } from '../../../../shared/enums/grid-button-style.enum';
import { JournalApiService } from '../../../../core/services/journal-api';

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
      label: 'View Journal',
      class: GridButtonStyle.AzureGridNeutralBtn,
      icon: 'summarize',
      action: 'viewJournalWithFeedback',
      tooltip: 'Journal & Feedback',
    },
    // {
    //   label: '',
    //   color: '#bdb6b6',
    //   icon: 'transform',
    //   action: 'activateOrDeactivate',
    //   tooltip: 'activate / deactivate',
    // },
  ];

  readonly pageSizeOptions: number[] = [10, 20, 30, 40, 50, 100];

  private userId!: string;

  private readonly router: Router = inject(Router);
  private readonly dialog: MatDialog = inject(MatDialog);
  private readonly orchestratorApi = inject(OrchestratorApiService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly taskApiService: TaskApiService = inject(TaskApiService);
  private readonly journalApiService = inject(JournalApiService);
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
    if (event.action === 'viewJournalWithFeedback') {
      this.viewJournalWithFeedback(event.row);
    } else if (event.action === 'activateOrDeactivate') {
      this.restoreOrSoftDelete(event.row);
    }
  }

  writeJournal(): void {
    this.openJournalDialog(null); // nothing to edit, so null passed
  }

  // To Do: Phase 2
  editJournal(): void {}

  private openJournalDialog(
    journal: JournalEntryWithTasksAndFeedbackDto | null,
  ): void {
    let tasksOfUser: TaskItemDto[] = [];
    let tasksToSelect: DropDownListItem[] = [];
    this.loadingService.show();
    const sub = this.taskApiService.getMyTasks(true).subscribe({
      next: (value: TaskItemDto[]) => {
        tasksOfUser = value;

        tasksToSelect = tasksOfUser.map((task) => ({
          value: task.id,
          label: task.title,
        }));

        this.loadingService.hide();

        if (tasksToSelect.length === 0) {
          this.snackbarService.info(
            'You dont have any task assigned to write journals on. Please Discuss with your manager',
            'Close',
          );
          return;
        }

        (document.activeElement as HTMLElement)?.blur();

        const dialogRef = this.dialog
          .open(AddEditJournalDialog, {
            width: '750px',
            minHeight: '750px',
            maxHeight: '90vh',
            panelClass: 'journal-dialog-panel',

            data: {
              journalToEdit: journal,
              tasksToSelect: tasksToSelect,
            },
          })
          .afterClosed()
          .subscribe((result) => {
            if (result) {
              // refresh table
              this.fetchAllUserJournals();
            }
          });
      },
      error: (err: any) => {
        console.error('Failed to fetch tasks list Of User', err);
        this.snackbarService.error(
          'Failed to fetch all user tasks for the logged in user !',
        );
        this.loadingService.hide();
      },
    });
    this.compositeSubscription.add(sub);
  }

  private viewJournalWithFeedback(
    journal: JournalEntryWithTasksAndFeedbackDto,
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
        if (result && journal.feedback?.id) {
          this.markJournalAsSeened(journal.feedback?.id);
        }
      });
  }

  private markJournalAsSeened(journalId: string): void {
    const sub = this.journalApiService
      .markJournalAsSeened(journalId)
      .subscribe({
        next: () => {
          this.loadingService.hide();
        },
        error: (err: any) => {
          console.error('Failed to mark journal as seened', err);
          this.snackbarService.error('Failed to to mark journal as seened !');
          this.loadingService.hide();
        },
      });
    this.compositeSubscription.add(sub);
  }

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
    this.loadingService.show();
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

  private addTestJournalWithTaskLinks(): void {
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

    // API call
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
}
