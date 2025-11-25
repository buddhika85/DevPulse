import { UserAccountDto } from './user-account.dto';

export interface UserProfileResponseDto {
  user: UserAccountDto;
  devPulseJwToken: string;
}
