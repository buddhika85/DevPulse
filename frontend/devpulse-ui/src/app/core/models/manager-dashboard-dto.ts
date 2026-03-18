import { LabelNumberDto, SummaryCardsDto } from './developer-dashboard-dto';

export interface ManagerDashboardDto {
  // summary - Cards
  summaryCardsDto: ManagerSummaryCardsDto;

  // Team Journals Per Developer (Bar)
  teamJournalsPerDeveloperBarChartDto: LabelNumberDto[];

  // Feedback Given vs Pending (Donut)
  feedbackDonutChartDto: FeedbackDonutChartDto;

  // Tasks Assigned vs Completed (Stacked Bar)
  tasksWithStatus: LabelNumberDto[];

  // last updated time
  lastUpdated: string;

  // other information
  managerId: string;
  managerDisplayName: string;
}

export interface FeedbackDonutChartDto {
  feedbackCompleted: number;
  feedbackPending: number;
}

export interface ManagerSummaryCardsDto {
  highPriorityCount: number;
  newTasksCount: number;
  inProgressTasksCount: number;
  urgentTasksCount: number;
  newJournalsNeedingFeedback: number;
}
