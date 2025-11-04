// src/app/msal-initializer.ts
import { MsalService } from '@azure/msal-angular';
import { Observable } from 'rxjs';

export function initializeMsal(
  msalService: MsalService
): () => Observable<void> {
  return () => msalService.initialize();
}
