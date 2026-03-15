using SharedLib.DTOs.Task;
using SharedLib.DTOs.User;

namespace SharedLib.DTOs.Journal
{
    public record TeamJournalEntryWithTasksAndFeedbackDto(Guid Id,
                                                            Guid UserId,
                                                            DateTime CreatedAt,
                                                            string Title,
                                                            string Content,
                                                            bool IsDeleted,
                                                            JournalFeedbackDto? feedback,
                                                            string? feedbackManager,
                                                            IReadOnlyList<TaskItemDto> LinkedTasks,
                                                            UserAccountDto User
                                                        ) : JournalEntryWithTasksAndFeedbackDto(Id,
                                                                                                UserId,
                                                                                                CreatedAt,
                                                                                                Title,
                                                                                                Content,
                                                                                                IsDeleted,
                                                                                                feedback,
                                                                                                feedbackManager,
                                                                                                LinkedTasks)
    {
        public string UserDisplayName => User.DisplayName;
    }
}
