export interface DeveloperDashboardDto {
  // summary - Cards
  summaryCardsDto: SummaryCardsDto;

  // Journals Over Time - Line chart
  journalsOverTimeLineChartDto: TimeSeriesPointDto[];

  // Task statuses count - Donut chart
  taskStatusesDonutChartDto: TaskStatusesCountsDto;

  // Feedback vs No Feedback - Bar chart
  journalFeedbackCountsBarChartDto: JournalFeedbackCountsDto;

  // last updated time
  lastUpdated: string;

  // other information
  userId: string;
  userDisplayName: string;
}

export interface TimeSeriesPointDto {
  label: string;
  value: number;
}

export interface SummaryCardsDto {
  highPriorityCount: number;
  newTasksCount: number;
  inProgressTasksCount: number;
  urgentTasksCount: number;
  newFeedbacksCount: number;
}

export interface TaskStatusesCountsDto {
  notStartedTaskCount: number;
  inProgressTaskCount: number;
  completedTaskCount: number;
}

export interface JournalFeedbackCountsDto {
  withFeedBackJournalCount: number;
  withoutFeedBackJournalCount: number;
  totalJounralCount: number;
}
