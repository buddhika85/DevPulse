export interface MoodFormDto {
  id: string | null;
  day: Date;

  moodTime: string;
  moodLevel: string;
  note: string | null;
}
