export const environment = {
  production: false,
  msal: {
    clientId: 'd3c23dbf-5662-4d7e-8c5c-c66436e3ad3a',
    tenantId: '6d90d58a-c9e3-4438-98a6-99ef5c8fc64b',
    redirectUri: 'http://localhost:4200',

    // ✅ Define scopes and URLs for each microservice
    protectedResources: {
      userApi: {
        url: 'https://localhost:7249/',
        scopes: ['api://user-service/access_as_user'],
      },
      taskApi: {
        url: 'https://localhost:7218/',
        scopes: ['api://task-service/access_as_user'],
      },

      // future APIs
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
