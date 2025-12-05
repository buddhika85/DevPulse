import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { AuthService } from './auth';
import { catchError, from, Observable, switchMap, throwError } from 'rxjs';
import { UserProfileResponseDto } from '../models/user-profile-response.dto';
import { UserAccountDto } from '../models/user-account.dto';
import { UserRole } from '../models/user-role.enum';

// dedicated for User Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  // user micro service URL
  private apiUrl = environment.msal.protectedResources.userApi.url;
  private profileControllerUrl = `${this.apiUrl}api/Profile`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  // This method returns an Observable that will eventually emit a UserProfile object
  // Whoever subscribes to getUserProfile() will receive the user profile once the token is acquired and the API responds.
  // In the backend it checks whether there is user with tokens oid,
  // if not it will create that user (new registration),
  // if available, it will query and take the user from SQL DB
  // Then User Dto will be returned for that user record

  getUserProfile(): Observable<UserProfileResponseDto> {
    return this.http.get<UserProfileResponseDto>(
      `${this.profileControllerUrl}/me`
    );
  }

  getUserById(id: string): Observable<UserAccountDto> {
    return this.http.get<UserAccountDto>(`${this.profileControllerUrl}/${id}`);
  }

  getAllUserProfiles(
    includeDeleted: boolean = false
  ): Observable<UserAccountDto[]> {
    // prepare query strings
    const queryString = new HttpParams().set('includeDeleted', includeDeleted);

    // send the call
    return this.http.get<UserAccountDto[]>(`${this.profileControllerUrl}/all`, {
      params: queryString,
    });
  }

  getUsersByRole(role: UserRole): Observable<UserAccountDto[]> {
    // prepare query strings
    const queryString = new HttpParams().set('role', role);

    // send the call
    return this.http.get<UserAccountDto[]>(
      `${this.profileControllerUrl}/by-role`,
      {
        params: queryString,
      }
    );
  }

  restoreUser(id: string): Observable<void> {
    return this.http.patch<void>(
      `${this.profileControllerUrl}/restore/${id}`,
      {}
    );
  }

  softDeleteUser(id: string): Observable<void> {
    return this.http.patch<void>(
      `${this.profileControllerUrl}/soft-delete/${id}`,
      {}
    );
  }
}
