import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { UserStoreService } from '../../../core/services/user-store.service';
import { CommonModule, JsonPipe } from '@angular/common';
import { UserRole } from '../../../core/models/user-role.enum';

@Component({
  selector: 'app-devtools',
  imports: [JsonPipe, CommonModule],
  templateUrl: './devtools.html',
  styleUrl: './devtools.scss',
})
export class Devtools {
  router = inject(Router);
  auth = inject(AuthService);
  userStore = inject(UserStoreService);

  msalUser = this.auth.getUser();
  userDto = this.userStore.userDto();

  private userFeatureFlags = {
    TaskLoger: true,
    EnableExport: true,
    ShowMoodTracker: true,
    JournalEntry: true,

    TeamDashboard: false,
    Feedback: false,
    GoalSetter: false,

    UserManagement: false,
    ApiLimits: false,
    SystemHealth: false,

    DevPanel: true,
  };

  private managerFeatureFlags = {
    TaskLoger: false,
    EnableExport: false,
    ShowMoodTracker: false,
    JournalEntry: false,

    TeamDashboard: true,
    Feedback: true,
    GoalSetter: true,

    UserManagement: false,
    ApiLimits: false,
    SystemHealth: false,

    DevPanel: true,
  };

  private adminFeatureFlags = {
    TaskLoger: false,
    EnableExport: false,
    ShowMoodTracker: false,
    JournalEntry: false,

    TeamDashboard: false,
    Feedback: false,
    GoalSetter: false,

    UserManagement: true,
    ApiLimits: true,
    SystemHealth: true,

    DevPanel: true,
  };

  private featureFlags = {
    TaskLoger: false,
    EnableExport: false,
    ShowMoodTracker: false,
    JournalEntry: false,

    TeamDashboard: false,
    Feedback: false,
    GoalSetter: false,

    UserManagement: false,
    ApiLimits: false,
    SystemHealth: false,

    DevPanel: false,
  };

  userFeatureFlagByRole =
    this.userDto?.userRole === null
      ? this.featureFlags
      : this.userDto?.userRole === UserRole.Admin
      ? this.adminFeatureFlags
      : this.userDto?.userRole === UserRole.Manager
      ? this.managerFeatureFlags
      : this.userFeatureFlags;
}
