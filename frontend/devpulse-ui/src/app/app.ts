import { Component, OnInit, signal } from '@angular/core';
import { MsalService } from '@azure/msal-angular';

@Component({
  standalone: false, // ✅ Required for module-based bootstrapping

  selector: 'app-root',
  //imports: [RouterOutlet],      // ❌ Only used in standalone: true

  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  protected readonly title = signal('devpulse-ui');

  constructor(private msal: MsalService) {}

  async ngOnInit(): Promise<void> {
    try {
      // Executed after redirect from Entra ID (Azure AD) following successful login
      // MSAL detects the authorization code in the URL fragment (#code=...)
      // MSAL sends this code to Entra ID to redeem tokens via the token endpoint
      // Entra ID responds with:
      // 1. ID token → identifies the signed-in user (used by frontend to display identity) - example to use userDto.displayName
      // 2. Access token → authorizes calls to protected backend APIs (e.g., /api/Profile/me)
      // 3. Refresh token → MSAL uses silent renew via iframe or redirect
      // MSAL caches these tokens and returns the login result
      const result = await this.msal.instance.handleRedirectPromise();

      if (result?.account) {
        // MSAL sets the active account
        // For future token acquisition and identity resolution
        // Stores ID token and access token in browser storage (localStorage)
        // Refresh tokens are not stored in SPAs — MSAL uses iframe or redirect-based silent renew instead
        this.msal.instance.setActiveAccount(result.account);
        console.log('✅ MSAL login complete:', result.account);
      }
    } catch (error) {
      console.error('❌ MSAL redirect handling failed:', error);
    }
  }
}
