export interface JournalFormDto {
  id: string | null;
  title: string | null;
  content: string | null;
  linkedTasks: string[];
  userId: string | null;
}
