using SharedLib.DTOs.ManagerDashboard;

public record DeveloperDashboardDto(
    SummaryCardsDto SummaryCardsDto,
    List<TimeSeriesPointDto> JournalsOverTimeLineChartDto,
    TaskStatusesCountsDto TaskStatusesDonutChartDto,
    JournalFeedbackCountsDto JournalFeedbackCountsBarChartDto,
    string LastUpdated,
    Guid UserId,
    string UserDisplayName
);

public record TimeSeriesPointDto(
    string Label,
    int Value
): LabelNumberDto(Label, Value);

public record SummaryCardsDto(
    int HighPriorityCount,
    int NewTasksCount,
    int InProgressTasksCount,
    int UrgentTasksCount,
    int NewFeedbacksCount
);

public record TaskStatusesCountsDto(
    int NotStartedTaskCount,
    int InProgressTaskCount,
    int CompletedTaskCount
);

public record JournalFeedbackCountsDto(
    int WithFeedBackJournalCount,
    int WithoutFeedBackJournalCount,
    int TotalJounralCount
);