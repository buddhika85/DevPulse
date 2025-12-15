import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { TaskItemDto } from '../models/task-item.dto';
import { HttpClient, HttpParams } from '@angular/common/http';

// dedicated for Task Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class TaskApiService {
  //micro service URL
  private readonly apiUrl = environment.msal.protectedResources.taskApi.url;
  private readonly profileControllerUrl = `${this.apiUrl}api/Tasks`;
  private readonly http: HttpClient = inject(HttpClient);

  getTasksByUserId(
    userId: string,
    includeDeleted: boolean
  ): Observable<TaskItemDto[]> {
    var queryString = new HttpParams().set('includeDeleted', includeDeleted);

    return this.http.get<TaskItemDto[]>(
      `${this.profileControllerUrl}/by-user/${userId}`,
      {
        params: queryString,
      }
    );
  }

  restoreTask(taskId: string): Observable<void> {
    return of();
  }

  softDeleteTask(taskId: string): Observable<void> {
    return of();
  }
}
