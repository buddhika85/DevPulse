using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedLib.Domain.Entities;

namespace JournalService.Infrastructure.Common.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context, ILogger logger)
        {
            try
            {
                // Find all tracked entities with in DB Context that have domain events
                var domainEntitiesWithEvents = context.ChangeTracker
                    .Entries<BaseEntity>()
                    .Where(x => x.Entity.DomainEvents.Any())
                    .ToList();

                // Extract all domain events from those entities
                var domainEvents = domainEntitiesWithEvents
                    .SelectMany(x => x.Entity.DomainEvents)
                    .ToList();

                // Publish each domain event via MediatR
                foreach (var domainEvent in domainEvents)
                {
                    logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);
                    await mediator.Publish(domainEvent);
                }

                // Clear domain events after publishing to avoid duplicate dispatch
                domainEntitiesWithEvents.ForEach(entity => entity.Entity.ClearDomainEvents());
            }
            catch (Exception ex)
            {
                // Log without assuming a specific domain event type
                logger.LogError(ex, "Failed to dispatch domain events from tracked entities.");
                throw;
            }
        }
    }
}
