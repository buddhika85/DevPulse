using OrchestratorService.Application.DTOs;
using SharedLib.DTOs.Journal;
using System.Net;
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

                var journalId = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);
                //if (journalId)
                //{
                //    _logger.LogWarning("Journal API returned invalid journalId for user ID: {UserId}", addJournalEntryDto.UserId);
                //    throw new ApplicationException("Journal API returned invalid journalId");
                //}

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

        public async Task<JournalEntryDto?> GetJournalByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching journal by ID: {JournalId}", id);

                var url = $"{RouteToJournalController}{id}"; // have trailing / in RouteToJournalController constant
                var response = await _httpClient.GetAsync(url, cancellationToken);        

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Journal not found: {JournalId}", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();     // throws exception if its not a success code

                var journal = await response.Content.ReadFromJsonAsync<JournalEntryDto>(cancellationToken: cancellationToken);
                return journal;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching journal by Id {JournalId}", id);
                throw new ApplicationException("Failed to fetch tasks from user API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for journal response for journal Id {JournalId}", id);
                throw new ApplicationException("Failed to deserialize journal data", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Journal fetching operation was canceled for journal Id {JournalId}", id);
                throw;
            }
        }


        // TO DO
        public Task DeleteJournalEntryAsync(Guid? jounralId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsJournalEntryExistsByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Executing JournalService API and Checking if a journal-entry exists by journal ID={JournalId} at {Time}", id, now);
            try
            {
                var url = $"{RouteToJournalController}is-exists/{id}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var isExists = await response.Content.ReadFromJsonAsync<bool?>(cancellationToken: cancellationToken);
                if (isExists is null)
                {
                    _logger.LogWarning("JournalService returned null for existence check of {JournalId}", id);
                    return false;
                }

                _logger.LogDebug("Journal exists={Exists} for {JournalId} at {Time}", isExists, id, now);
                return isExists.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if a journal-entry exists by journal ID={JournalId} at {Time}", id, now);
                throw;
            }
        }

        public async Task<bool> UpdateJournalEntryAsync(UpdateJournalEntryDto dto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Executing JournalService API and updating journal with ID={JournalId} at {Time}", dto.JournalEntryId, now);
            try
            {
                var url = $"{RouteToJournalController}update/{dto.JournalEntryId}";
                var response = await _httpClient.PatchAsJsonAsync(url, dto, cancellationToken);
                response.EnsureSuccessStatusCode();         // 204 - no content is expected

                _logger.LogInformation("Successfully updated journal {JournalId} at {Time}",
                                    dto.JournalEntryId, DateTime.UtcNow);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Updating journal with ID={JournalId} at {Time} was Unsuccessful", dto.JournalEntryId, now);
                throw;
            }
        }
    }
}
