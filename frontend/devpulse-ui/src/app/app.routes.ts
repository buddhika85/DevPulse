import { Routes } from '@angular/router';

// Define the application's route configuration
export const routes: Routes = [
  {
    path: '', // Root path of the app
    loadComponent: () => import('./layout/layout').then((m) => m.Layout), // Load shell layout component
    children: [
      // Nested routes rendered inside the layout
      {
        path: 'profile', // URL: /profile
        loadComponent: () =>
          import('./features/user-profile/user-profile').then(
            (m) => m.UserProfile // Lazy-load UserProfile component
          ),
      },
      {
        path: 'admin', // URL: /admin
        loadComponent: () =>
          import('./features/admin-dashboard/admin-dashboard').then(
            (m) => m.AdminDashboard // Lazy-load AdminDashboard component
          ),
      },
      {
        path: '', // Empty child path
        redirectTo: 'profile', // Redirect to /profile by default
        pathMatch: 'full', // Match full path to trigger redirect
      },
    ],
  },
];
