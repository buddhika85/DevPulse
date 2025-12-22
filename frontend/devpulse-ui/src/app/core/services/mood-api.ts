import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MoodEntryDto } from '../models/mood-entry.dto';
import { AddMoodEntryDto } from '../models/add-mood-entry.dto';
import { UpdateMoodEntryDto } from '../models/update-mood-entry.dto';

// dedicated for Mood Micro Service Calls

@Injectable({
  providedIn: 'root',
})
export class MoodApiService {
  //micro service URL
  private readonly apiUrl = environment.msal.protectedResources.moodApi.url;
  private readonly moodControllerUrl = `${this.apiUrl}api/Mood`;
  private readonly http: HttpClient = inject(HttpClient);

  getMoodsByUserId(userId: string): Observable<MoodEntryDto[]> {
    return this.http.get<MoodEntryDto[]>(
      `${this.moodControllerUrl}/by-user/${userId}`
    );
  }

  getMoodById(id: string): Observable<MoodEntryDto> {
    return this.http.get<MoodEntryDto>(`${this.moodControllerUrl}/${id}`);
  }

  // checking before insert
  isMoodEntryExists(
    userId: string,
    day: string,
    time: string
  ): Observable<boolean> {
    var queryString = new HttpParams().set('day', day).set('time', time);

    return this.http.get<boolean>(
      `${this.moodControllerUrl}/is-exists/${userId}`,
      {
        params: queryString,
      }
    );
  }

  findOtherMoodEntry(
    excludedId: string,
    userId: string,
    day: string,
    time: string
  ): Observable<boolean> {
    var queryString = new HttpParams()
      .set('excludedId', excludedId)
      .set('day', day)
      .set('time', time);

    return this.http.get<boolean>(
      `${this.moodControllerUrl}/find-other/${userId}`,
      {
        params: queryString,
      }
    );
  }

  createMood(addMoodEntry: AddMoodEntryDto): Observable<void> {
    return this.http.post<void>(`${this.moodControllerUrl}`, addMoodEntry);
  }

  updateMood(id: string, updatedMood: UpdateMoodEntryDto): Observable<void> {
    return this.http.patch<void>(
      `${this.moodControllerUrl}/update/${id}`,
      updatedMood
    );
  }

  deleteMood(id: string): Observable<void> {
    return this.http.delete<void>(`${this.moodControllerUrl}/delete/${id}`, {});
  }
}
