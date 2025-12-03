import { Component } from '@angular/core';
import { GenericTableComponent } from '../../../../core/shared/components/generic-table.component/generic-table.component';

@Component({
  selector: 'app-user-management',
  imports: [GenericTableComponent],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss',
})
export class UserManagement {
  taskColumns = [
    { key: 'id', label: 'ID' },
    { key: 'title', label: 'Title' },
    { key: 'status', label: 'Status' },
  ];

  taskActions = [
    { label: 'Edit', color: 'accent', icon: 'edit', action: 'edit' },
    { label: 'Delete', color: 'warn', icon: 'delete', action: 'delete' },
  ];

  tasks = [
    { id: 1, title: 'Write blog post', status: 'Open' },
    { id: 2, title: 'Fix bug #123', status: 'In Progress' },
    { id: 3, title: 'Write blog post', status: 'Open' },
    {
      id: 4,
      title:
        "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
      status: 'In Progress',
    },
    { id: 5, title: 'Write blog post', status: 'Open' },
    { id: 6, title: 'Fix bug #123', status: 'In Progress' },
  ];

  handleTaskAction(event: { action: string; row: any }) {
    alert(`Action Triggered ${event.action} on ${event.row}`);
  }
}
