export interface AddMoodEntryDto {
  userId: string; // Guid
  day?: string | null; // nullable DateTime
  moodTime?: string | null;
  moodLevel?: string | null;
  note?: string | null;
}
