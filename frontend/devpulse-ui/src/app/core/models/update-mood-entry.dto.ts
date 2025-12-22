export interface UpdateMoodEntryDto {
  id: string; // Guid
  day: string; // DateTime (non-null)
  moodTime: string;
  moodLevel: string;
  note?: string | null; // nullable
}
