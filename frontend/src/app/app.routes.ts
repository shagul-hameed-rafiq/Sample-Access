import { Routes } from '@angular/router';
import { AppShell } from './core/layout/app-shell/app-shell';

export const routes: Routes = [
  {
    path: '',
    component: AppShell,
    children: [
      { path: '', redirectTo: 'members', pathMatch: 'full' },

      {
        path: 'members',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/members/members-list/members-list').then((m) => m.MembersList),
          },

          {
            path: 'add',
            loadComponent: () =>
              import('./features/members/add-member/add-member').then((m) => m.AddMember),
          },

          {
            path: 'edit/:id',
            loadComponent: () =>
              import('./features/members/add-member/add-member').then((m) => m.AddMember),
          },

          {
            path: ':id',
            children: [
              {
                path: '',
                loadComponent: () =>
                  import('./features/members/member-details/member-details').then(
                    (m) => m.MemberDetails,
                  ),
              },


              {
                path: 'reports/add',
                loadComponent: () =>
                  import('./features/reports/add-report-intake/add-report-intake').then(
                    (m) => m.AddReportIntake,
                  ),
              },


              {
                path: 'reports/tests/:panelId',
                loadComponent: () =>
                  import('./features/reports/test-entry/test-entry').then(
                    (m) => m.TestEntryComponent,
                  ),
              },

              {
                path: 'reports/preview/:visitId',
                loadComponent: () =>
                  import('./features/reports/report-preview/report-preview').then(
                    (m) => m.ReportPreview,
                  ),
              },


              {
                path: 'reports/view/:reportId',
                loadComponent: () =>
                  import('./features/reports/report-view/report-view').then((m) => m.ReportView),
              },
            ],
          },
        ],
      },
    ],
  },
];
