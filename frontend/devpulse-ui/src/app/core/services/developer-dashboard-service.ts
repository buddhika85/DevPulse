import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import {
  DeveloperDashboardDto,
  JournalFeedbackCountsDto,
  SummaryCardsDto,
  TaskStatusesCountsDto,
  TimeSeriesPointDto,
} from '../models/developer-dashboard-dto';

@Injectable({
  providedIn: 'root',
})
export class DeveloperDashboardService {
  private apiUrl = environment.msal.protectedResources.orchestratorApi.url;
  private dashboardControllerUrl = `${this.apiUrl}api/dashboard`;

  constructor(private http: HttpClient) {}

  getMyDashbaord(userId: string): Observable<DeveloperDashboardDto> {
    return this.http.get<DeveloperDashboardDto>(
      `${this.dashboardControllerUrl}/developer/${userId}`,
    );

    //return of(this.getMockTestData(userId));
  }

  private getMockTestData(userId: string): DeveloperDashboardDto {
    // summary
    const summaryCardsDto: SummaryCardsDto = {
      highPriorityCount: 1,
      newTasksCount: 3,
      inProgressTasksCount: 2,
      urgentTasksCount: 1,
      newFeedbacksCount: 2,
    };

    // Journals Over Time - Line chart
    const journalsOverTimeLineChartDto: TimeSeriesPointDto[] = [
      { label: '202-12-01', value: 1 },
      { label: '202-12-02', value: 2 },
      { label: '202-12-03', value: 1 },
      { label: '202-12-04', value: 3 },
      { label: '202-12-05', value: 1 },
    ];

    // Task statuses count - Donut chart
    const taskStatusesDonutChartDto: TaskStatusesCountsDto = {
      notStartedTaskCount: 1,
      inProgressTaskCount: 2,
      completedTaskCount: 3,
    };

    // Feedback vs No Feedback - Bar chart
    const journalFeedbackCountsBarChartDto: JournalFeedbackCountsDto = {
      withFeedBackJournalCount: 3,
      withoutFeedBackJournalCount: 1,
      totalJounralCount: 4,
    };

    const dto: DeveloperDashboardDto = {
      // summary
      summaryCardsDto: summaryCardsDto,

      // Journals Over Time - Line chart
      journalsOverTimeLineChartDto: journalsOverTimeLineChartDto,

      // Task statuses count - Donut chart
      taskStatusesDonutChartDto: taskStatusesDonutChartDto,

      // Feedback vs No Feedback - Bar chart
      journalFeedbackCountsBarChartDto: journalFeedbackCountsBarChartDto,

      // last updated time
      lastUpdated: new Date().toLocaleString(),

      // other
      userId: userId,
      userDisplayName: 'Developer Test',
    };
    return dto;
  }
}
