import { BaseDto } from './base.dto';

export interface TaskItemDto extends BaseDto {
  title: string;
  description: string;
  status: string;
  createdAt: Date;
  createdAtStr: string;
  isDeleted: boolean;
  isDeletedStr: string;

  userId: string;
  priority: string;
  dueDate: Date | null;
}
