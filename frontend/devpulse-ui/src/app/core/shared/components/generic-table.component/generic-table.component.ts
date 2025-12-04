import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { TableColumn } from '../../../models/table-column';
import { TableAction } from '../../../models/table-action';
import {
  MatCellDef,
  MatColumnDef,
  MatHeaderCellDef,
} from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatIcon } from '@angular/material/icon'; // ✅ Import Material icons
import { MatFormField, MatLabel } from '@angular/material/input';
import { MatTableDataSource } from '@angular/material/table';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-generic-table',
  imports: [
    MatTableModule, // table
    MatPaginatorModule, // paginator
    MatSortModule, // sorting
    MatButtonModule, // action buttons
    MatIconModule, // icons
    MatFormFieldModule, // <mat-form-field>
    MatInputModule, // <input matInput>
    MatTooltipModule,
  ],
  templateUrl: './generic-table.component.html',
  styleUrl: './generic-table.component.scss',
})
export class GenericTableComponent implements AfterViewInit {
  @Input() columns: TableColumn[] = [];
  @Input() actions: TableAction[] = [];

  // Data input → updates the MatTableDataSource
  @Input() set data(value: any[]) {
    this.dataSource.data = value || [];
    // If paginator is already available, re‑attach it
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  // Emit action events back to parent
  @Output() actionTriggered = new EventEmitter<{ action: string; row: any }>();

  // Angular Material data source wrapper
  dataSource = new MatTableDataSource<any>([]);

  // Build displayed columns dynamically (data columns + actions column)
  displayedColumns(): string[] {
    return this.actions.length > 0
      ? [...this.columns.map((c) => c.key), 'actions']
      : [...this.columns.map((c) => c.key)];
  }

  // Paginator and Sort references
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // Hook paginator and sort into dataSource after view init
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    // Defer sort activation to next tick
    setTimeout(() => {
      if (this.columns.length > 0) {
        this.sort.active = this.columns[0].key;
        this.sort.direction = 'asc';
        this.sort.sortChange.emit(); // trigger re-sort
      }
    });
  }

  // Apply filter (search box)
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  // Emit action event when a button is clicked
  onAction(action: string, row: any) {
    this.actionTriggered.emit({ action, row });
  }
}
