import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell'; // ✅ Layout wrapper for all child routes

// ✅ Define the main route configuration for DevPulse
// SHELL component is eager loaded, eveything else are child compoenents and they are lazy loaded inside router outlet of Shell
export const routes: Routes = [
  {
    path: '', // ✅ Root path of the app (e.g., http://localhost:4200/)
    component: Shell, // ✅ Shell component acts as consistent toolbar layout (for top nav bar)

    // ✅ Child routes rendered inside <router-outlet> of Shell Lazy Loaded
    children: [
      {
        path: '', // ✅ Default child route (e.g., dashboard)
        loadComponent: () =>
          import('./features/dashboard/dashboard').then((m) => m.Dashboard), // ✅ Lazy-load standalone Dashboard component
      },

      // 🔹 Developer Routes
      {
        path: 'tasks',
        loadComponent: () =>
          import('./features/developer/tasks/task-logger/task-logger').then(
            (m) => m.TaskLogger
          ),
        data: { roles: ['Developer'] },
      },
      {
        path: 'mood',
        loadComponent: () =>
          import('./features/developer/mood/mood-tracker/mood-tracker').then(
            (m) => m.MoodTracker
          ),
        data: { roles: ['Developer'] },
      },
      {
        path: 'journal',
        loadComponent: () =>
          import(
            './features/developer/journal/journal-entry/journal-entry'
          ).then((m) => m.JournalEntry),
        data: { roles: ['Developer'] },
      },
      {
        path: 'export',
        loadComponent: () =>
          import(
            './features/developer/summary/summary-export/summary-export'
          ).then((m) => m.SummaryExport),
        data: { roles: ['Developer'] },
      },

      // 🔹 Manager Routes
      {
        path: 'team-dashboard',
        loadComponent: () =>
          import(
            './features/manager/team-dashboard/team-dashboard/team-dashboard'
          ).then((m) => m.TeamDashboard),
        data: { roles: ['Manager'] },
      },
      {
        path: 'feedback',
        loadComponent: () =>
          import('./features/manager/feedback/feedback/feedback').then(
            (m) => m.Feedback
          ),
        data: { roles: ['Manager'] },
      },
      {
        path: 'goals',
        loadComponent: () =>
          import('./features/manager/goals/goal-setter/goal-setter').then(
            (m) => m.GoalSetter
          ),
        data: { roles: ['Manager'] },
      },

      // 🔹 Admin Routes
      {
        path: 'users',
        loadComponent: () =>
          import(
            './features/admin/user-management/user-management/user-management'
          ).then((m) => m.UserManagement),
        data: { roles: ['Admin'] },
      },
      {
        path: 'limits',
        loadComponent: () =>
          import('./features/admin/api-limits/api-limits/api-limits').then(
            (m) => m.ApiLimits
          ),
        data: { roles: ['Admin'] },
      },
      {
        path: 'health',
        loadComponent: () =>
          import(
            './features/admin/system-health/system-health/system-health'
          ).then((m) => m.SystemHealth),
        data: { roles: ['Admin'] },
      },
    ],
  },

  // 🔹 Unauthorized
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./features/errors/unauthorized/unauthorized').then(
        (m) => m.Unauthorized
      ),
  },

  // ✅ Wildcard route: redirect unknown paths to root
  { path: '**', redirectTo: '' },
];
