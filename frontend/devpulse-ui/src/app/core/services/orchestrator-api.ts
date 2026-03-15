import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import {
  AdminDashboardDto,
  UserDashboardDto,
} from '../models/base-dashbaord.dto';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ManagerDashboard } from '../../features/manager/manager-dashboard/manager-dashboard';
import { TaskItemWithUserDto } from '../models/task-item.dto';
import {
  CreateJournalDto,
  JournalEntryWithTasksAndFeedbackDto,
  TeamJournalEntryWithTasksAndFeedbackDto,
} from '../models/journal-entry-with-tasks-and-feedback.dto';

// dedicated for Orchestrator Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class OrchestratorApiService {
  // orchestratorApi micro service URL
  private apiUrl = environment.msal.protectedResources.orchestratorApi.url;
  private dashbaordControllerUrl = `${this.apiUrl}api/dashboard`;
  private tasksControllerUrl = `${this.apiUrl}api/tasks`;
  private orchestratorJournalsControllerUrl = `${this.apiUrl}api/journals`;

  constructor(private http: HttpClient) {}

  getAdminDashBaord(userId: string): Observable<AdminDashboardDto> {
    return this.http.get<AdminDashboardDto>(
      `${this.dashbaordControllerUrl}/${userId}`,
    );
  }

  getManagerDashBaord(userId: string): Observable<ManagerDashboard> {
    return this.http.get<ManagerDashboard>(
      `${this.dashbaordControllerUrl}/${userId}`,
    );
  }

  getDeveloperDashBaord(userId: string): Observable<UserDashboardDto> {
    return this.http.get<UserDashboardDto>(
      `${this.dashbaordControllerUrl}/${userId}`,
    );
  }

  getManagedTasks(includeDeleted: boolean): Observable<TaskItemWithUserDto[]> {
    var queryString = new HttpParams().set('includeDeleted', includeDeleted);

    return this.http.get<TaskItemWithUserDto[]>(
      `${this.tasksControllerUrl}/tasks-for-team`,
      {
        params: queryString,
      },
    );
  }

  getMyJournals(
    includeDeleted: boolean,
  ): Observable<JournalEntryWithTasksAndFeedbackDto[]> {
    var queryString = new HttpParams().set('includeDeleted', includeDeleted);

    return this.http.get<JournalEntryWithTasksAndFeedbackDto[]>(
      `${this.orchestratorJournalsControllerUrl}/my-journals`,
      {
        params: queryString,
      },
    );
  }

  getTeamJournals(
    includeDeleted: boolean,
  ): Observable<TeamJournalEntryWithTasksAndFeedbackDto[]> {
    var queryString = new HttpParams().set('includeDeleted', includeDeleted);

    return this.http.get<TeamJournalEntryWithTasksAndFeedbackDto[]>(
      `${this.orchestratorJournalsControllerUrl}/team-journals`,
      {
        params: queryString,
      },
    );
  }

  addJournalWithTaskLinks(dto: CreateJournalDto): Observable<void> {
    return this.http.post<void>(
      `${this.orchestratorJournalsControllerUrl}`,
      dto,
    );
  }
}
