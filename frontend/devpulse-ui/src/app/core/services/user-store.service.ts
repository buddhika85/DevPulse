import { Injectable, signal } from '@angular/core';
import { UserAccountDto } from '../models/user-account.dto';

// This is to share loggged in users details such as role (for route gurads and display route based on correct role)
@Injectable({
  providedIn: 'root',
})
export class UserStoreService {
  private _userDto = signal<UserAccountDto | null>(null);

  setUserDto(dto: UserAccountDto | null) {
    this._userDto.set(dto);
  }

  userDto() {
    return this._userDto();
  }
}
