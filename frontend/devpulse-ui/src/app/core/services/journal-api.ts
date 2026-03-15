import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

// dedicated for Journal Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class JournalApiService {
  private apiUrl = environment.msal.protectedResources.journalApi.url;
  private journalControllerUrl = `${this.apiUrl}api/Journal`;
  private feebackControllerUrl = `${this.apiUrl}api/JournalFeedback`;

  constructor(private http: HttpClient) {}

  markJournalAsSeened(journalId: string): Observable<void> {
    return this.http.patch<void>(
      `${this.feebackControllerUrl}/mark-as-seened/${journalId}`,
      {},
    );
  }
}
