import { Injectable } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { AccountInfo } from '@azure/msal-browser';
import { environment } from '../../../environments/environment';
import { ProtectedResource, ResourceKey } from '../models/msal-config';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private msal: MsalService) {}

  // ✅ Trigger login using redirect
  login(): void {
    this.msal.loginRedirect({
      scopes: environment.msal.protectedResources.userApi.scopes,
    });
  }

  // ✅ Trigger logout and redirect to home
  logout(): void {
    // ✅ Clear MSAL session (Entra token + account info)
    this.msal.logoutRedirect({
      postLogoutRedirectUri: environment.msal.redirectUri,
    });
  }

  // ✅ Get active user account info
  getUser(): AccountInfo | null {
    return this.msal.instance.getActiveAccount() ?? null;
  }

  // ✅ getAllAccounts() returns an array of signed-in accounts - as per local browser state
  isLoggedIn(): boolean {
    return this.msal.instance.getAllAccounts().length > 0;
  }

  // ✅ Get access token for a specific API
  async getAccessToken(resourceUrl: string): Promise<string | null> {
    const scopes = this.getScopesForResource(resourceUrl);
    if (!scopes) return null;

    try {
      const result = await this.msal.instance.acquireTokenSilent({
        account: this.getUser()!,
        scopes,
      });
      return result.accessToken;
    } catch (error) {
      console.error('Token acquisition failed', error);
      return null;
    }
  }

  // ✅ Helper: Get scopes for a given API URL
  private getScopesForResource(resourceUrl: string): string[] | null {
    const entries = Object.entries(environment.msal.protectedResources) as [
      ResourceKey,
      ProtectedResource
    ][];
    for (const [key, resource] of entries) {
      if (resource.url === resourceUrl) {
        return resource.scopes;
      }
    }
    return null;
  }
}
