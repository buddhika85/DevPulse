export interface TaskFormDto {
  id: string | null;
  title: string | null;
  description: string | null;
  status: string;
  priority: string;
  dueDate: Date;
}
