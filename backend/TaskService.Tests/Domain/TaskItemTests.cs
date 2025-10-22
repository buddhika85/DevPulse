using MediatR;
using TaskService.Domain.Entities;
using TaskService.Domain.Events;

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
            var title = "Title";
            var description = "Description";

            // act
            var result = TaskItem.Create(title, description);

            // assert
            result.Should().NotBeNull();
            result.Title.Should().Be(title);
            result.Description.Should().Be(description);
            result.TaskStatus.Should().Be(TaskService.Domain.ValueObjects.TaskStatus.Pending);

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
            var taskItem = TaskItem.Create("Title", "Description");
            var newTitle = "New Title";
            var newDescription = "New Description";
            var newStatus = TaskService.Domain.ValueObjects.TaskStatus.Completed;

            // act
            taskItem.Update(newTitle, newDescription, newStatus);

            // assert
            taskItem.Title.Should().Be(newTitle);
            taskItem.Description.Should().Be(newDescription);
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

        // Update_
        // SoftDelete_WhenCalled_SetsIsDeletedAndRaises...
    }
}
