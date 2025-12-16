import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { TaskItemDto } from '../models/task-item.dto';
import { HttpClient, HttpParams } from '@angular/common/http';
import { UpdateTaskDto } from '../models/update-task.dto';
import { CreateTaskDto } from '../models/create-task.dto';

// dedicated for Task Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class TaskApiService {
  //micro service URL
  private readonly apiUrl = environment.msal.protectedResources.taskApi.url;
  private readonly taskControllerUrl = `${this.apiUrl}api/Tasks`;
  private readonly http: HttpClient = inject(HttpClient);

  getTasksByUserId(
    userId: string,
    includeDeleted: boolean
  ): Observable<TaskItemDto[]> {
    var queryString = new HttpParams().set('includeDeleted', includeDeleted);

    return this.http.get<TaskItemDto[]>(
      `${this.taskControllerUrl}/by-user/${userId}`,
      {
        params: queryString,
      }
    );
  }

  getTaskById(id: string): Observable<TaskItemDto> {
    return this.http.get<TaskItemDto>(`${this.taskControllerUrl}/${id}`);
  }

  restoreTask(id: string): Observable<void> {
    return this.http.patch<void>(`${this.taskControllerUrl}/restore/${id}`, {});
  }

  softDeleteTask(id: string): Observable<void> {
    return this.http.patch<void>(
      `${this.taskControllerUrl}/soft-delete/${id}`,
      {}
    );
  }

  createTask(createTask: CreateTaskDto): Observable<void> {
    return this.http.post<void>(`${this.taskControllerUrl}`, createTask);
  }

  updateTask(id: string, updatedTask: UpdateTaskDto): Observable<void> {
    return this.http.patch<void>(
      `${this.taskControllerUrl}/${id}`,
      updatedTask
    );
  }
}
