import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UserAccountDto } from '../../../core/models/user-account.dto';
import { UserStoreService } from '../../../core/services/user-store.service';
import { interval, of, Subscription } from 'rxjs';
import { LoadingService } from '../../../core/services/loading-service';

import { NgxApexchartsModule } from 'ngx-apexcharts';
import {
  ChartOptionsLine,
  ChartOptionsDonut,
  ChartOptionsBar,
} from '../../../core/models/charting-types';
import { AsyncPipe } from '@angular/common';
import { DeveloperDashboardService } from '../../../core/services/developer-dashboard-service';
import {
  DeveloperDashboardDto,
  JournalFeedbackCountsDto,
  SummaryCardsDto,
  TaskStatusesCountsDto,
  TimeSeriesPointDto,
} from '../../../core/models/developer-dashboard-dto';

@Component({
  selector: 'app-developer-dashboard',
  standalone: true,
  imports: [NgxApexchartsModule, AsyncPipe],
  templateUrl: './developer-dashboard.html',
  styleUrl: './developer-dashboard.scss',
})
export class DeveloperDashboard implements OnInit, OnDestroy {
  private readonly compositeSubscription = new Subscription();

  private readonly loadingService = inject(LoadingService);
  private readonly userStoreService = inject(UserStoreService);
  private readonly devDashboardService = inject(DeveloperDashboardService);

  private userDto: UserAccountDto | null = null;
  developerDashboardDto: DeveloperDashboardDto | null = null;

  lastUpdated = new Date().toLocaleTimeString();
  loading = false;

  private refreshSub!: Subscription;

  // Summary card observable
  summary$ = of({
    highPriority: 0,
    newTasks: 0,
    inProgress: 0,
    urgent: 0,
    newFeedback: 0,
  });

  // Chart initial configs
  journalsLineChart: ChartOptionsLine = {
    series: [{ name: 'Journals', data: [] }],
    chart: { type: 'line', height: 300 },
    xaxis: { categories: [] },
    stroke: { curve: 'smooth' },
    yaxis: {
      min: 0, // start from 0
      forceNiceScale: true, // clean rounded ticks
      decimalsInFloat: 0, // no decimal values
    },
  };

  tasksDonutChart: ChartOptionsDonut = {
    series: [0, 0, 0],
    chart: { type: 'donut', height: 260 },
    labels: ['Completed', 'In Progress', 'Not Started'],
    colors: ['#0078d4', '#ffaa44', '#d9534f'],
  };

  feedbackBarChart: ChartOptionsBar = {
    series: [{ name: 'Feedback', data: [0, 0, 0] }],
    chart: { type: 'bar', height: 300 },
    xaxis: { categories: ['Total', 'With Feedback', 'Without Feedback'] },
    colors: ['#6a5acd'],
  };

  // Lifecycle
  ngOnInit(): void {
    this.loading = true;
    this.fetchDeveloperDashboardData();

    // Auto-refresh every 30 seconds
    this.refreshSub = interval(30000).subscribe(() => {
      if (!this.loading) {
        this.fetchDeveloperDashboardData();
      }
    });
  }

  private fetchDeveloperDashboardData(): void {
    this.loading = true;
    this.loadingService.show();

    this.userDto = this.userStoreService.userDto();
    if (!this.userDto) return;

    const sub = this.devDashboardService
      .getMyDashbaord(this.userDto.id)
      .subscribe({
        next: (dto: DeveloperDashboardDto) => {
          this.developerDashboardDto = dto;
          //console.log('Developer Dashbaord', this.developerDashboardDto);

          // Update UI sections
          this.setSummaryCardData();
          this.setJournalsOverTimeLineChartData();
          this.setTasksDonutChartData();
          this.setFeedbackBarChartData();

          this.lastUpdated = dto.lastUpdated;

          this.loading = false;
          this.loadingService.hide();
        },
        error: (err) => {
          console.error('Failed to load developer dashboard', err);
          this.loading = false;
          this.loadingService.hide();
        },
      });

    this.compositeSubscription.add(sub);
  }

  // Summary cards
  private setSummaryCardData(): void {
    const dto: SummaryCardsDto | undefined =
      this.developerDashboardDto?.summaryCardsDto;
    if (!dto) return;

    this.summary$ = of({
      highPriority: dto.highPriorityCount,
      newTasks: dto.newTasksCount,
      inProgress: dto.inProgressTasksCount,
      urgent: dto.urgentTasksCount,
      newFeedback: dto.newFeedbacksCount,
    });
  }

  // Line chart (Journals Over Time)
  private setJournalsOverTimeLineChartData(): void {
    const dto: TimeSeriesPointDto[] | undefined =
      this.developerDashboardDto?.journalsOverTimeLineChartDto;
    if (!dto) return;

    this.journalsLineChart = {
      ...this.journalsLineChart,
      series: [{ name: 'Journals', data: dto.map((x) => x.value) }],
      xaxis: { categories: dto.map((x) => x.label) },
    };
  }

  // Donut chart (Task statuses)
  private setTasksDonutChartData(): void {
    const dto: TaskStatusesCountsDto | undefined =
      this.developerDashboardDto?.taskStatusesDonutChartDto;
    if (!dto) return;

    this.tasksDonutChart = {
      ...this.tasksDonutChart,
      series: [
        dto.completedTaskCount,
        dto.inProgressTaskCount,
        dto.notStartedTaskCount,
      ],
    };
  }

  // Bar chart (Feedback counts)
  private setFeedbackBarChartData(): void {
    const dto = this.developerDashboardDto?.journalFeedbackCountsBarChartDto;
    if (!dto) return;

    this.feedbackBarChart = {
      ...this.feedbackBarChart,
      series: [
        {
          name: 'Feedback',
          data: [
            dto.totalJounralCount,
            dto.withFeedBackJournalCount,
            dto.withoutFeedBackJournalCount,
          ],
        },
      ],
    };
  }

  // Cleanup
  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
    this.refreshSub?.unsubscribe();
  }
}
