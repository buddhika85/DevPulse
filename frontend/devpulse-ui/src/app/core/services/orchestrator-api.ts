import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import {
  AdminDashboardDto,
  UserDashboardDto,
} from '../models/base-dashbaord.dto';
import { HttpClient } from '@angular/common/http';
import { ManagerDashboard } from '../../features/manager/manager-dashboard/manager-dashboard';

// dedicated for Orchestrator Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class OrchestratorApiService {
  // orchestratorApi micro service URL
  private apiUrl = environment.msal.protectedResources.orchestratorApi.url;
  private dashbaordControllerUrl = `${this.apiUrl}api/dashboard/`;

  constructor(private http: HttpClient) {}

  getAdminDashBaord(userId: string): Observable<AdminDashboardDto> {
    return this.http.get<AdminDashboardDto>(
      `${this.dashbaordControllerUrl}${userId}`
    );
  }

  getManagerDashBaord(userId: string): Observable<ManagerDashboard> {
    return this.http.get<ManagerDashboard>(
      `${this.dashbaordControllerUrl}${userId}`
    );
  }

  getDeveloperDashBaord(userId: string): Observable<UserDashboardDto> {
    return this.http.get<UserDashboardDto>(
      `${this.dashbaordControllerUrl}${userId}`
    );
  }
}
