import { BaseDto } from './base.dto';

export interface TaskItemDto extends BaseDto {
  title: string;
  description: string;
  status: string;
  createdAt: Date;

  userId: string;
  priority: string;
  dueDate: Date | null;
}
