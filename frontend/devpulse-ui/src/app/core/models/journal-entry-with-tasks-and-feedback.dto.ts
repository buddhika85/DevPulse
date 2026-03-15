import { BaseDto } from './base.dto';
import { TaskItemDto } from './task-item.dto';
import { UserAccountDto } from './user-account.dto';

export interface JournalEntryWithTasksAndFeedbackDto extends BaseDto {
  //id: string; // Guid coming from BaseDto
  idSnippet: string;
  userId: string; // Guid
  createdAt: string; // ISO date string
  title: string;
  content: string;
  isDeleted: boolean;

  feedback: JournalFeedbackDto | null;
  feedbackManager: string | null;

  linkedTasks: TaskItemDto[];

  // Computed fields (sent from backend)
  isDeletedStr: string;
  createdAtStr: string;
  isFeedbackGiven: boolean;
  isFeedbackGivenStr: string;
  isFeedbackSeenByUser: string;
  linkedTaskTitles: string[];
  linkedTaskTitlesCsv: string;
  contentSnippet: string;
}

export interface TeamJournalEntryWithTasksAndFeedbackDto extends JournalEntryWithTasksAndFeedbackDto {
  user: UserAccountDto;

  // Computed fields (sent from backend)
  userDisplayName: string;
}

export interface JournalFeedbackDto extends BaseDto {
  //id: string; // Guid coming from BaseDto
  journalEntryId: string;
  feedbackManagerId: string;
  comment: string;
  createdAt: string;
  seenByUser: boolean;

  // Computed
  createdAtStr: string;
  seenByUserStr: string;
}

export interface CreateJournalDto {
  addJournalEntryDto: AddJournalEntryDto;
  linkedTaskIds: string[]; // HashSet<Guid> → string[]
}

export interface AddJournalEntryDto {
  userId: string; // Guid → string
  title: string;
  content: string;
}
