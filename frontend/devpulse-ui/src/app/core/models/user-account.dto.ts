import { BaseDto } from './base.dto';
import { UserRole } from './user-role.enum';

export interface UserAccountDto extends BaseDto {
  displayName: string;
  email: string;
  userRole: UserRole; // restrict to known roles of backend
  createdAt: string;
}
