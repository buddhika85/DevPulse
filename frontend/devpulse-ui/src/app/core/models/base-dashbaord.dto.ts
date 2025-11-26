import { UserAccountDto } from './user-account.dto';

export interface BaseDashboardDto {
  user: UserAccountDto;
  dashBoardType: string;
}

export interface AdminDashboardDto extends BaseDashboardDto {
  // To Do: add Admin specific dashbord properties
}

export interface ManagerDashboardDto extends BaseDashboardDto {
  // To Do: add Manager specific dashbord properties
}

export interface UserDashboardDto extends BaseDashboardDto {
  // To Do: add User (developer) specific dashbord properties
}
