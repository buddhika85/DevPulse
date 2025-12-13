import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { TaskItemDto } from '../models/task-item.dto';
import { HttpClient } from '@angular/common/http';

// dedicated for Task Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class TaskApiService {
  //micro service URL
  private readonly apiUrl = environment.msal.protectedResources.taskApi.url;
  private readonly profileControllerUrl = `${this.apiUrl}api/Tasks`;
  private readonly http: HttpClient = inject(HttpClient);

  getTasksByUserId(userId: string): Observable<TaskItemDto[]> {
    return this.http.get<TaskItemDto[]>(
      `${this.profileControllerUrl}/by-user/${userId}`
    );
  }
}
