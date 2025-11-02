export const environment = {
  production: true,
  msal: {
    clientId: '',
    tenantId: '',
    redirectUri: '',
    apiScope: '',

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
