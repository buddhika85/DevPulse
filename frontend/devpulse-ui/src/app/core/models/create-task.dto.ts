export interface CreateTaskDto {
  userId: string;
  title: string | null;
  description: string | null;
  dueDate: Date | null;
  status: string | null; // defaults to "NotStarted" on backend
  priority: string | null; // defaults to "Low" on backend
}
