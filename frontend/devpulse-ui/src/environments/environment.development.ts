import { MsalConfig } from '../app/core/models/msal-config';

export const environment: { production: boolean; msal: MsalConfig } = {
  production: false,
  msal: {
    clientId: '99e041f2-daf9-4cd2-8723-7ae039b10478',
    tenantId: '6d90d58a-c9e3-4438-98a6-99ef5c8fc64b',
    redirectUri: 'http://localhost:4200',
    apiScope: 'api://91761975-cb30-43bd-9b13-32cd76d65aad/access_as_user', // ✅ This must match exactly what you configured in Expose an API → Scopes in the Azure portal for your backend app registration.
    authority:
      'https://login.microsoftonline.com/6d90d58a-c9e3-4438-98a6-99ef5c8fc64b/v2.0',

    // ✅ Define scopes and URLs for each microservice
    protectedResources: {
      userApi: {
        url: 'https://localhost:7249/',
        scopes: ['api://91761975-cb30-43bd-9b13-32cd76d65aad/access_as_user'],
      },
      taskApi: {
        url: 'https://localhost:7218/',
        scopes: ['api://91761975-cb30-43bd-9b13-32cd76d65aad/access_as_user'],
      },

      // future APIs
      moodApi: {
        url: '',
        scopes: ['api://91761975-cb30-43bd-9b13-32cd76d65aad/access_as_user'],
      },
      journalApi: {
        url: '',
        scopes: ['api://91761975-cb30-43bd-9b13-32cd76d65aad/access_as_user'],
      },
      dashboardApi: {
        url: '',
        scopes: ['api://91761975-cb30-43bd-9b13-32cd76d65aad/access_as_user'],
      },
      tenantApi: {
        url: '',
        scopes: ['api://91761975-cb30-43bd-9b13-32cd76d65aad/access_as_user'],
      },
    },
  },
};
