using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedLib.Domain.Entities;

namespace TaskService.Infrastructure.Common.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context)
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
                await mediator.Publish(domainEvent);
            }

            // Clear domain events after publishing to avoid duplicate dispatch
            domainEntitiesWithEvents.ForEach(entity => entity.Entity.ClearDomainEvents());

        }
    }
}
