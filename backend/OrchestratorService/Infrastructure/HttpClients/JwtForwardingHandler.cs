using System.Net.Http.Headers;

namespace OrchestratorService.Infrastructure.HttpClients
{
    /// <summary>
    /// DelegatingHandler that forwards the caller's JWT token
    /// from the current HttpContext into outgoing HttpClient requests.
    /// This ensures downstream microservices receive the same
    /// Authorization header as the orchestrator did.
    /// </summary>
    public class JwtForwardingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor: inject IHttpContextAccessor so we can read the current request's headers
        public JwtForwardingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Intercepts outgoing HTTP requests.
        /// If the incoming request had an Authorization header,
        /// copy it into the outgoing request so the JWT is forwarded.
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Get current HttpContext (represents the incoming request to the orchestrator)
            var httpContext = _httpContextAccessor.HttpContext;

            // Try to read the Authorization header from the incoming request
            var token = httpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(token))
            {
                // Ensure the header is in "Bearer <token>" format
                if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = $"Bearer {token}";
                }

                // Attach the Authorization header to the outgoing request
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
            }

            // Continue with the pipeline (send request to microservice)
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
