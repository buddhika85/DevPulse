import { BaseDto } from './base.dto';

export interface TaskItemDto extends BaseDto {
  title: string;
  description: string;
  status: string;
  createdAt: Date;
  isDeleted: boolean;

  userId: string;
  priority: string;
  dueDate: Date | null;
}
