import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth';
import { catchError, from, Observable, switchMap, throwError } from 'rxjs';
import { UserProfileResponseDto } from '../models/user-profile-response.dto';
import { UserAccountDto } from '../models/user-account.dto';

// dedicated for User Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  // user micro service URL
  private apiUrl = environment.msal.protectedResources.userApi.url;
  private profileControllerUrl = `${this.apiUrl}api/Profile/`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  // This method returns an Observable that will eventually emit a UserProfile object
  // Whoever subscribes to getUserProfile() will receive the user profile once the token is acquired and the API responds.
  // In the backend it checks whether there is user with tokens oid,
  // if not it will create that user (new registration),
  // if available, it will query and take the user from SQL DB
  // Then User Dto will be returned for that user record

  getUserProfile(): Observable<UserProfileResponseDto> {
    return this.http.get<UserProfileResponseDto>(
      `${this.profileControllerUrl}me`
    );
  }

  getAllUserProfiles(): Observable<UserAccountDto[]> {
    return this.http.get<UserAccountDto[]>(`${this.profileControllerUrl}all`);
  }
}
