import { Component, inject, Inject } from '@angular/core';
import { UserStoreService } from '../../core/services/user-store.service';
import { DeveloperDashboard } from '../developer/developer-dashboard/developer-dashboard';
import { ManagerDashboard } from '../manager/manager-dashboard/manager-dashboard';
import { AdminDashboard } from '../admin/admin-dashboard/admin-dashboard';

@Component({
  selector: 'app-dashboard',
  imports: [DeveloperDashboard, ManagerDashboard, AdminDashboard],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  userStoreService: UserStoreService = inject(UserStoreService);
}
