using SharedLib.DTOs.Journal;
using System.Text.Json;

namespace OrchestratorService.Infrastructure.HttpClients.JournalMicroService
{
    public class JournalServiceClient : IJournalServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JournalServiceClient> _logger;

        private const string RouteToJournalController = "api/journal/";

        public JournalServiceClient(HttpClient httpClient, ILogger<JournalServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Guid?> AddJournalEntryAsync(AddJournalEntryDto addJournalEntryDto, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Adding journal for user ID: {UserId}", addJournalEntryDto.UserId);

                var response = await _httpClient.PostAsJsonAsync(
                    RouteToJournalController,
                    addJournalEntryDto,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Journal API returned {StatusCode}. Body: {Body}",
                        response.StatusCode, errorBody);

                    throw new ApplicationException($"Journal API failed with status {response.StatusCode}");
                }

                var journalIdStr = await response.Content.ReadAsStringAsync(cancellationToken);
                if (journalIdStr is null)
                {
                    _logger.LogWarning("Journal API returned null journalId for user ID: {UserId}", addJournalEntryDto.UserId);
                    throw new ApplicationException("Journal API returned null journalId");
                }

                if (!Guid.TryParse(journalIdStr, out Guid journalId))
                {
                    _logger.LogWarning("Journal API returned invalid journalId for user ID: {UserId}", addJournalEntryDto.UserId);
                    throw new ApplicationException("Journal API returned invalid journalId");
                }

                _logger.LogInformation("Successfully created journal entry for user ID: {UserId}", addJournalEntryDto.UserId);
                return journalId;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while adding journal for user ID: {UserId}", addJournalEntryDto.UserId);
                throw new ApplicationException("Failed to create journal entry via Journal API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for user ID: {UserId}", addJournalEntryDto.UserId);
                throw new ApplicationException("Failed to deserialize Journal API response", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Journal entry creation was canceled for user ID: {UserId}", addJournalEntryDto.UserId);
                throw;
            }
        }

        public Task DeleteJournalEntryAsync(Guid? jounralId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
