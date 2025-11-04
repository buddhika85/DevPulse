import { MsalConfig } from '../app/core/models/msal-config';

export const environment: { production: boolean; msal: MsalConfig } = {
  production: true,
  msal: {
    clientId: '',
    tenantId: '',
    redirectUri: '',
    apiScope: '', // ✅ This must match exactly what you configured in Expose an API → Scopes in the Azure portal for your backend app registration.
    authority:
      'https://login.microsoftonline.com/6d90d58a-c9e3-4438-98a6-99ef5c8fc64b/v2.0',

    // ✅ Define scopes and URLs for each microservice
    protectedResources: {
      userApi: {
        url: '',
        scopes: ['api://user-service/access_as_user'],
      },
      taskApi: {
        url: '',
        scopes: ['api://task-service/access_as_user'],
      },

      moodApi: {
        url: '',
        scopes: ['api://mood-service/access_as_user'],
      },
      journalApi: {
        url: '',
        scopes: ['api://journal-service/access_as_user'],
      },
      dashboardApi: {
        url: '',
        scopes: ['api://dashboard-service/access_as_user'],
      },
      tenantApi: {
        url: '',
        scopes: ['api://tenant-service/access_as_user'],
      },
    },
  },
};
