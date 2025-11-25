import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth';
import { catchError, from, Observable, switchMap, throwError } from 'rxjs';
import { UserProfileResponseDto } from '../models/user-profile-response.dto';

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
    // // from converts async promise returned by getAccessToken
    // return from(this.authService.getAccessToken(this.apiUrl)).pipe(
    //   // switchMap takes the emitted token and switches to a new Observable
    //   // Because we want to cancel any previous token requests if a new one comes in (e.g., rapid reloads).

    //   switchMap((token) => {
    //     // We create an Authorization header with the token, so the backend knows who the user is.

    //     const headers = new HttpHeaders({
    //       Authorization: `Bearer ${token}`,
    //     });

    //     // We make a GET request to https://localhost:7249/api/Profile/me

    //     return this.http.get<UserAccountDto>(`${this.profileControllerUrl}me`, {
    //       headers,
    //     });
    //   }),
    //   catchError((error) => {
    //     console.error('Failed to fetch user profile', error);
    //     return throwError(() => error);
    //   })
    // );

    return this.http.get<UserProfileResponseDto>(
      `${this.profileControllerUrl}me`
    );
  }
}
