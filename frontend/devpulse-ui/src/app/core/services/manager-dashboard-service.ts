import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { SummaryCardsDto } from '../models/developer-dashboard-dto';
import {
  ManagerDashboardDto,
  ManagerSummaryCardsDto,
} from '../models/manager-dashboard-dto';

@Injectable({
  providedIn: 'root',
})
export class ManagerDashboardService {
  private apiUrl = environment.msal.protectedResources.orchestratorApi.url;
  private dashboardControllerUrl = `${this.apiUrl}api/dashboard`;

  constructor(private http: HttpClient) {}

  getMyDashbaord(managerId: string): Observable<ManagerDashboardDto> {
    return this.http.get<ManagerDashboardDto>(
      `${this.dashboardControllerUrl}/manager/${managerId}`,
    );

    //return of(this.getMockTestData(managerId));
  }

  getMockTestData(managerId: string): any {
    // summary
    const summaryCardsDto: ManagerSummaryCardsDto = {
      highPriorityCount: 1,
      newTasksCount: 3,
      inProgressTasksCount: 2,
      urgentTasksCount: 1,
      newJournalsNeedingFeedback: 2,
    };

    const dto: ManagerDashboardDto = {
      // summary
      summaryCardsDto: summaryCardsDto,

      // Chart 1: Team Journals Per Developer (Bar)
      teamJournalsPerDeveloperBarChartDto: [
        { value: 1, label: 'Dev A' },
        { value: 3, label: 'Dev B' },
        { value: 2, label: 'Dev C' },
      ],
      // Chart 2: Feedback Given vs Pending (Donut)
      feedbackDonutChartDto: {
        feedbackCompleted: 5,
        feedbackPending: 2,
      },

      // Chart 3: Tasks Assigned vs Completed (Stacked Bar)
      tasksWithStatus: [
        { value: 1, label: 'Assigned' },
        { value: 3, label: 'Completed' },
        { value: 2, label: 'Overdue' },
      ],

      // last updated time
      lastUpdated: new Date().toLocaleString(),

      // other
      managerId: managerId,
      managerDisplayName: 'Manager Test',
    };
    return dto;
  }
}
