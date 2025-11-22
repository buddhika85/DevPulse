using MediatR;
using Microsoft.VisualBasic;
using TaskService.Domain.Entities;
using TaskService.Domain.Events;
using TaskService.Domain.ValueObjects;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TaskService.Tests.Domain
{
    // Tests TaskItem entity class
    // MethodName_StateUnderTest_ExpectedBehavior

    public class TaskItemTests          
    {
        [Fact]
        public void Create_WithValidInput_ReturnsInitializedTaskItemAndRaisesCreatedEvent()
        {
            // arrange
            var userId = Guid.NewGuid();
            var title = "Title";
            var description = "Description";
            var dueDate = DateTime.UtcNow.AddDays(7).Date;
            var priority = TaskPriority.Medium;
            var status = TaskService.Domain.ValueObjects.TaskStatus.Pending;

            // act
            var result = TaskItem.Create(userId, title, description, dueDate, priority, status);

            // assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Title.Should().Be(title);
            result.Description.Should().Be(description);
            result.DueDate.Should().Be(dueDate);
            result.TaskPriority.Should().Be(TaskPriority.Medium);
            result.TaskStatus.Should().Be(TaskService.Domain.ValueObjects.TaskStatus.Pending);

            result.DomainEvents.Should().HaveCount(1);          // one event for creation of task
            result.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<TaskCreatedDomainEvent>();
            var @event = result.DomainEvents.Should().ContainSingle()
                                .Which.As<TaskCreatedDomainEvent>();

            @event.Should().NotBeNull();
            @event.TaskCreated.Title.Should().Be(title);
            @event.TaskCreated.Description.Should().Be(description);
            @event.TaskCreated.TaskStatus.Should().Be(TaskService.Domain.ValueObjects.TaskStatus.Pending);
        }

        [Fact]
        public void Update_WithNewValues_ChangesStateAndRaisesUpdatedEvent()
        {
            // arrange
            var userId = Guid.NewGuid();
            var title = "Title";
            var description = "Description";
            var dueDate = DateTime.UtcNow.AddDays(7).Date;
            var priority = TaskPriority.Medium;
            var status = TaskService.Domain.ValueObjects.TaskStatus.Pending;

            var taskItem = TaskItem.Create(userId, title, description, dueDate, priority, status);

            var newTitle = "New Title";
            var newDescription = "New Description";
            var newStatus = TaskService.Domain.ValueObjects.TaskStatus.Completed;
            var newPriority = TaskPriority.Low;
            var newDueDate = DateTime.UtcNow.Date;

            // act
            taskItem.Update(newTitle, newDescription, newStatus, newPriority, newDueDate);

            // assert
            taskItem.Should().NotBeNull();
            taskItem.UserId.Should().Be(userId);                                    // userId cannot be changed
            taskItem.Title.Should().Be(newTitle);
            taskItem.Description.Should().Be(newDescription);
            taskItem.DueDate.Should().Be(newDueDate);
            taskItem.TaskPriority.Should().Be(newPriority);
            taskItem.TaskStatus.Should().Be(newStatus);

            taskItem.DomainEvents.Should().HaveCount(2);                                  // 2 events: one for creation, one for update        
            var events = taskItem.DomainEvents.Should().BeOfType<List<INotification>>().Subject;

            var createdEvent = events.OfType<TaskCreatedDomainEvent>().Single();
            createdEvent.TaskCreated.Title.Should().Be(newTitle);
            createdEvent.TaskCreated.Description.Should().Be(newDescription);
            createdEvent.TaskCreated.TaskStatus.Should().Be(TaskService.Domain.ValueObjects.TaskStatus.Completed);

            var updatedEvent = events.OfType<TaskUpdatedDomainEvent>().Single();
            updatedEvent.TaskUpdated.Title.Should().Be(newTitle);
            updatedEvent.TaskUpdated.Description.Should().Be(newDescription);
            updatedEvent.TaskUpdated.TaskStatus.Should().Be(TaskService.Domain.ValueObjects.TaskStatus.Completed);
        }

        // SoftDelete_WhenCalled_SetsIsDeletedAndRaises...

        // Update_

    }
}
