export interface ProtectedResource {
  url: string;
  scopes: string[];
}

export type ResourceKey =
  | 'userApi'
  | 'taskApi'
  | 'orchestratorApi'
  | 'moodApi'
  | 'journalApi'
  | 'taskJournalListApi';

export interface MsalConfig {
  clientId: string;
  tenantId: string;
  redirectUri: string;
  apiScope: string;
  authority: string;
  protectedResources: Record<ResourceKey, ProtectedResource>;
}
