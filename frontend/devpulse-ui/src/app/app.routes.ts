import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell'; // ✅ Layout wrapper for all child routes
import { roleguardGuard } from './core/guards/roleguard-guard';

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

      // 🔹 User Routes
      {
        path: 'tasks',
        loadComponent: () =>
          import('./features/developer/tasks/task-logger/task-logger').then(
            (m) => m.TaskLogger
          ),
        data: { roles: ['User'] },
        canActivate: [roleguardGuard],
      },
      {
        path: 'mood',
        loadComponent: () =>
          import('./features/developer/mood/mood-tracker/mood-tracker').then(
            (m) => m.MoodTracker
          ),
        data: { roles: ['User'] },
        canActivate: [roleguardGuard],
      },
      {
        path: 'journal',
        loadComponent: () =>
          import(
            './features/developer/journal/journal-entry/journal-entry'
          ).then((m) => m.JournalEntry),
        data: { roles: ['User'] },
        canActivate: [roleguardGuard],
      },
      {
        path: 'export',
        loadComponent: () =>
          import(
            './features/developer/summary/summary-export/summary-export'
          ).then((m) => m.SummaryExport),
        data: { roles: ['User'] },
        canActivate: [roleguardGuard],
      },

      // 🔹 Manager Routes
      {
        path: 'team-dashboard',
        loadComponent: () =>
          import(
            './features/manager/team-dashboard/team-dashboard/team-dashboard'
          ).then((m) => m.TeamDashboard),
        data: { roles: ['Manager'] },
        canActivate: [roleguardGuard],
      },
      {
        path: 'feedback',
        loadComponent: () =>
          import('./features/manager/feedback/feedback/feedback').then(
            (m) => m.Feedback
          ),
        data: { roles: ['Manager'] },
        canActivate: [roleguardGuard],
      },
      {
        path: 'goals',
        loadComponent: () =>
          import('./features/manager/goals/goal-setter/goal-setter').then(
            (m) => m.GoalSetter
          ),
        data: { roles: ['Manager'] },
        canActivate: [roleguardGuard],
      },

      // 🔹 Admin Routes
      {
        path: 'users',
        loadComponent: () =>
          import(
            './features/admin/user-management/user-management/user-management'
          ).then((m) => m.UserManagement),
        data: { roles: ['Admin'] },
        canActivate: [roleguardGuard],
      },
      {
        path: 'limits',
        loadComponent: () =>
          import('./features/admin/api-limits/api-limits/api-limits').then(
            (m) => m.ApiLimits
          ),
        data: { roles: ['Admin'] },
        canActivate: [roleguardGuard],
      },
      {
        path: 'health',
        loadComponent: () =>
          import(
            './features/admin/system-health/system-health/system-health'
          ).then((m) => m.SystemHealth),
        data: { roles: ['Admin'] },
        canActivate: [roleguardGuard],
      },

      // 🔹 For Developmnent -  a hidden route for debugging, inspecting identity
      {
        path: 'devtools',
        loadComponent: () =>
          import('./features/devtools/devtools/devtools').then(
            (m) => m.Devtools
          ),
        data: { roles: ['Admin', 'Manager', 'User'] },
        canActivate: [roleguardGuard],
      },

      // 🔹 Unauthorized
      {
        path: 'unauthorized',
        loadComponent: () =>
          import('./features/errors/unauthorized/unauthorized').then(
            (m) => m.Unauthorized
          ),
      },
    ],
  },

  // ✅ Wildcard route: redirect unknown paths to root
  { path: '**', redirectTo: '' },
];
