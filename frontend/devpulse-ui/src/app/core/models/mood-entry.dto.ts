import { BaseDto } from './base.dto';

export interface MoodEntryDto extends BaseDto {
  day: string; // ISO string or date string
  moodTime: string;
  moodTimeRange: string;
  moodLevel: string;
  moodScore: number;
  note: string;
  createdAt: string; // ISO string or date string
  userId: string; // Guid

  dayStr: string; // computed on backend
  createdAtStr: string; // computed on backend
}
