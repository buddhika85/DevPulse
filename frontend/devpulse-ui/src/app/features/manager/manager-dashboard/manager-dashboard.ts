import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { interval, of, Subscription } from 'rxjs';
import { UserAccountDto } from '../../../core/models/user-account.dto';
import { UserStoreService } from '../../../core/services/user-store.service';
import { LoadingService } from '../../../core/services/loading-service';
import {
  ChartOptionsBar,
  ChartOptionsDonut,
} from '../../../core/models/charting-types';
import { NgxApexchartsModule } from 'ngx-apexcharts';
import { AsyncPipe } from '@angular/common';
import {
  FeedbackDonutChartDto,
  ManagerDashboardDto,
  ManagerSummaryCardsDto,
} from '../../../core/models/manager-dashboard-dto';
import { ManagerDashboardService } from '../../../core/services/manager-dashboard-service';
import {
  LabelNumberDto,
  SummaryCardsDto,
} from '../../../core/models/developer-dashboard-dto';

@Component({
  selector: 'app-manager-dashboard',
  imports: [NgxApexchartsModule, AsyncPipe],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.scss',
})
export class ManagerDashboard implements OnInit, OnDestroy {
  private readonly compositeSubscription: Subscription = new Subscription();

  private loadingService = inject(LoadingService);
  private userStoreService = inject(UserStoreService);
  private managerDashboardService = inject(ManagerDashboardService);

  private userDto: UserAccountDto | null = null;
  managerDashboardDto: ManagerDashboardDto | null = null;

  lastUpdated = new Date().toLocaleTimeString();
  loading = false;

  private refreshSub!: Subscription;

  // Summary Cards (Mock Data)
  summary$ = of({
    highPriority: 0,
    newTasks: 0,
    inProgress: 0,
    urgent: 0,
    newJournalsNeedingFeedback: 0,
  });

  // Chart 1: Team Journals Per Developer (Bar)
  teamJournalsBarChart: ChartOptionsBar = {
    series: [
      {
        name: 'Journals',
        data: [],
      },
    ],
    chart: { type: 'bar', height: 300 },
    xaxis: { categories: [] },
    colors: ['#3b82f6'], // blue
  };

  // Chart 2: Feedback Given vs Pending (Donut)
  feedbackDonutChart: ChartOptionsDonut = {
    series: [0, 0],
    chart: { type: 'donut', height: 300 },
    labels: ['Feedback Completed', 'Feedback Pending'],
    colors: ['#10b981', '#ef4444'], // green, red
  };

  // Chart 3: Tasks Assigned vs Completed (Stacked Bar)
  teamTasksStackedBarChart: ChartOptionsBar = {
    series: [
      // { name: 'Assigned', data: [10] },
      // { name: 'Completed', data: [5] },
      // { name: 'Overdue', data: [1] },
    ],
    chart: {
      type: 'bar',
      height: 300,
      stacked: true,
    },
    xaxis: { categories: ['Team Tasks'] },
    colors: ['#3b82f6', '#10b981', '#ef4444'], // blue, green, red
  };

  ngOnInit(): void {
    this.loading = true;

    this.fetchManagerDashboardData();

    // Auto-refresh every 60 seconds
    this.refreshSub = interval(60000).subscribe(() => {
      if (!this.loading) {
        this.fetchManagerDashboardData();
      }
    });
  }

  private fetchManagerDashboardData(): void {
    this.loading = true;
    this.loadingService.show();

    this.userDto = this.userStoreService.userDto();
    if (!this.userDto) return;

    const sub = this.managerDashboardService
      .getMyDashbaord(this.userDto.id)
      .subscribe({
        next: (value: ManagerDashboardDto) => {
          this.managerDashboardDto = value;
          console.log('ManagerDashboardDto', this.managerDashboardDto);
          this.setUpSummaryCardData();
          this.setUpTeamJournalsPerDeveloperBarChart();
          this.setUpFeedbackDonutChart();
          this.setupTasksWithStatusStackedBarChart();

          this.lastUpdated = this.managerDashboardDto.lastUpdated;

          this.loadingService.hide();
          this.loading = false;
        },
        error: (err) => {
          console.error('Failed to load manager dashboard', err);
          this.loadingService.hide();
          this.loading = false;
        },
      });

    this.compositeSubscription.add(sub);
  }

  private setupTasksWithStatusStackedBarChart(): void {
    const dto: LabelNumberDto[] | undefined =
      this.managerDashboardDto?.tasksWithStatus;
    if (!dto) return;
    this.teamTasksStackedBarChart = {
      ...this.teamTasksStackedBarChart,
      series: dto.map((x) => ({
        name: x.label,
        data: [x.value],
      })),

      // series: [
      //   { name: 'Assigned', data: [10] },
      //   { name: 'Completed', data: [5] },
      //   { name: 'Overdue', data: [1] },
      // ],
    };
  }

  private setUpFeedbackDonutChart(): void {
    const dto: FeedbackDonutChartDto | undefined =
      this.managerDashboardDto?.feedbackDonutChartDto;
    if (!dto) return;
    this.feedbackDonutChart = {
      ...this.feedbackDonutChart,
      series: [dto.feedbackCompleted, dto.feedbackPending],
    };
  }

  private setUpTeamJournalsPerDeveloperBarChart(): void {
    const dto: LabelNumberDto[] | undefined =
      this.managerDashboardDto?.teamJournalsPerDeveloperBarChartDto;
    if (!dto) return;
    this.teamJournalsBarChart = {
      ...this.teamJournalsBarChart,
      series: [{ name: 'Journals', data: dto.map((x) => x.value) }],
      xaxis: { categories: dto.map((x) => x.label) },
    };
  }

  private setUpSummaryCardData(): void {
    const dto: ManagerSummaryCardsDto | undefined =
      this.managerDashboardDto?.summaryCardsDto;
    if (!dto) return;

    this.summary$ = of({
      highPriority: dto.highPriorityCount,
      newTasks: dto.newTasksCount,
      inProgress: dto.inProgressTasksCount,
      urgent: dto.urgentTasksCount,
      newJournalsNeedingFeedback: dto.newJournalsNeedingFeedback,
    });
  }

  // Cleanup
  ngOnDestroy(): void {
    this.compositeSubscription.unsubscribe();
    this.refreshSub?.unsubscribe();
  }
}
