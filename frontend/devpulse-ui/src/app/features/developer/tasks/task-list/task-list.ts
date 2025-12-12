import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-task-list',
  imports: [],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss',
})
export class TaskList {
  private router: Router = inject(Router);

  add(): void {
    this.router.navigate(['tasks/add']);
  }

  edit(id: number): void {
    this.router.navigate(['tasks/edit', id]);
  }
}
