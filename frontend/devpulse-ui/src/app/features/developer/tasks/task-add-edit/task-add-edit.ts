import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-task-add-edit',
  imports: [],

  templateUrl: './task-add-edit.html',
  styleUrl: './task-add-edit.scss',
})
export class TaskAddEdit implements OnInit {
  private readonly activatedRoute: ActivatedRoute = inject(ActivatedRoute);
  private isAddMode: boolean = true;
  private editId: string | null = null;

  testMessage: string = '';

  ngOnInit(): void {
    this.editId = this.activatedRoute.snapshot.paramMap.get('id');
    this.isAddMode = this.editId === null;
    this.testMessage = `Add: ${this.isAddMode} and edit id: ${this.editId}`;
  }
}
