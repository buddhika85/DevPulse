export interface UpdateTaskDto {
  id: string;
  title: string;
  description: string;
  dueDate: Date | null;
  status: string; // defaults to "Pending" on backend
  priority: string; // defaults to "Low" on backend
}
