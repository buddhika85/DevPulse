export interface AddJournalFeedbackDto {
  jounralEntryId: string; // Guid
  feedbackManagerId: string; // Guid
  comment: string;
}
