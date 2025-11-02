import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell'; // ✅ Layout wrapper for all child routes

// ✅ Define the main route configuration for DevPulse
export const routes: Routes = [
  {
    path: '', // ✅ Root path of the app (e.g., http://localhost:4200/)
    component: Shell, // ✅ Shell component acts as layout (e.g., toolbar, sidenav)

    // ✅ Child routes rendered inside <router-outlet> of Shell
    children: [
      {
        path: '', // ✅ Default child route (e.g., dashboard)
        loadComponent: () =>
          import('./features/dashboard/dashboard').then((m) => m.Dashboard), // ✅ Lazy-load standalone Dashboard component
      },
    ],
  },

  // ✅ Wildcard route: redirect unknown paths to root
  { path: '**', redirectTo: '' },
];
