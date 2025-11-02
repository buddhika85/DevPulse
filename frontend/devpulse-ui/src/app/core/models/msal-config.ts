export interface ProtectedResource {
  url: string;
  scopes: string[];
}

export type ResourceKey =
  | 'userApi'
  | 'taskApi'
  | 'moodApi'
  | 'journalApi'
  | 'dashboardApi'
  | 'tenantApi';

export interface MsalConfig {
  clientId: string;
  tenantId: string;
  redirectUri: string;
  apiScope: string;
  protectedResources: Record<ResourceKey, ProtectedResource>;
}
