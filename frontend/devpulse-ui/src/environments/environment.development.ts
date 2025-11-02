import { MsalConfig } from '../app/core/models/msal-config';

export const environment: { production: boolean; msal: MsalConfig } = {
  production: false,
  msal: {
    clientId: 'd3c23dbf-5662-4d7e-8c5c-c66436e3ad3a',
    tenantId: '6d90d58a-c9e3-4438-98a6-99ef5c8fc64b',
    redirectUri: 'http://localhost:4200',
    apiScope: 'api://d3c23dbf-5662-4d7e-8c5c-c66436e3ad3a/access_as_user', // ✅ This must match exactly what you configured in Expose an API → Scopes in the Azure portal for your backend app registration.

    // ✅ Define scopes and URLs for each microservice
    protectedResources: {
      userApi: {
        url: 'https://localhost:7249/',
        scopes: ['api://d3c23dbf-5662-4d7e-8c5c-c66436e3ad3a/access_as_user'],
      },
      taskApi: {
        url: 'https://localhost:7218/',
        scopes: ['api://d3c23dbf-5662-4d7e-8c5c-c66436e3ad3a/access_as_user'],
      },

      // future APIs
      moodApi: {
        url: '',
        scopes: ['api://d3c23dbf-5662-4d7e-8c5c-c66436e3ad3a/access_as_user'],
      },
      journalApi: {
        url: '',
        scopes: ['api://d3c23dbf-5662-4d7e-8c5c-c66436e3ad3a/access_as_user'],
      },
      dashboardApi: {
        url: '',
        scopes: ['api://d3c23dbf-5662-4d7e-8c5c-c66436e3ad3a/access_as_user'],
      },
      tenantApi: {
        url: '',
        scopes: ['api://d3c23dbf-5662-4d7e-8c5c-c66436e3ad3a/access_as_user'],
      },
    },
  },
};
