namespace SharedLib.DTOs.ManagerDashboard
{
    public record ManagerDashboardDto(
    ManagerSummaryCardsDto SummaryCardsDto,
    List<LabelNumberDto> TeamJournalsPerDeveloperBarChartDto,
    FeedbackDonutChartDto FeedbackDonutChartDto,
    List<LabelNumberDto> TasksWithStatus,
    string LastUpdated,
    Guid ManagerId,
    string ManagerDisplayName);

    public record ManagerSummaryCardsDto(
        int HighPriorityCount,
        int NewTasksCount,
        int InProgressTasksCount,
        int UrgentTasksCount,
        int NewJournalsNeedingFeedback
    );

    public record FeedbackDonutChartDto(
        int FeedbackCompleted,
        int FeedbackPending
    );

    public record LabelNumberDto(
        string Label,
        int Value
    );
}
