import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

// dedicated for Journal Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class JournalApiService {
  private apiUrl = environment.msal.protectedResources.journalApi.url;
}
