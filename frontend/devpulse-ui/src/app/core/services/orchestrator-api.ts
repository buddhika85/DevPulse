import { Injectable, OnDestroy } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
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
import { DeveloperDashboardDto } from '../models/developer-dashboard-dto';

// dedicated for Orchestrator Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class OrchestratorApiService implements OnDestroy {
  // orchestratorApi micro service URL
  private apiUrl = environment.msal.protectedResources.orchestratorApi.url;
  private dashbaordControllerUrl = `${this.apiUrl}api/dashboard`;
  private tasksControllerUrl = `${this.apiUrl}api/tasks`;
  private orchestratorJournalsControllerUrl = `${this.apiUrl}api/journals`;

  // for cancellation in .NET API calls - cancellation token
  private abortController = new AbortController();

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

  /*
    This method now supports cancellation.

    - Angular uses AbortController to cancel HTTP requests.
    - When the user navigates away, ngOnDestroy() will fire.
    - abortController.abort() will cancel the HTTP request.
    - Browser closes the connection.
    - ASP.NET Core receives the disconnect and triggers HttpContext.RequestAborted.
    - That CancellationToken flows into your .NET controller and services.
  */

  getMyJournals(
    includeDeleted: boolean,
  ): Observable<JournalEntryWithTasksAndFeedbackDto[]> {
    var queryString = new HttpParams().set('includeDeleted', includeDeleted);

    return this.http.get<JournalEntryWithTasksAndFeedbackDto[]>(
      `${this.orchestratorJournalsControllerUrl}/my-journals`,
      {
        params: queryString,
        //signal: this.abortController.signal, // <-- cancellation support, TO DO: we need to update Angular Core package to get this to work
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

  /*
    Services CAN implement OnDestroy.
    Angular will call ngOnDestroy() when the service is destroyed
    (typically when the module providing it is destroyed).

    This is where we cancel any in-flight HTTP requests.
  */
  ngOnDestroy(): void {
    this.abortController.abort(); // <-- triggers cancellation
  }
}
