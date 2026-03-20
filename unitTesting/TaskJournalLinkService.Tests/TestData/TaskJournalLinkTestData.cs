using SharedLib.UnitTesting;
using TaskJournalLinkService.Domain.Models;

namespace TaskJournalLinkService.Tests.TestData
{
    public class TaskJournalLinkTestData : BaseTestData
    {
        public override IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            { 
                new TaskJournalLinkDocument(new Guid(), new Guid(), "Task 1", DateTime.UtcNow) 
            };
            yield return new object[]
            {
                new TaskJournalLinkDocument(new Guid(), new Guid(), "Task 2", DateTime.UtcNow.AddDays(-1))
            };
        }
    }
}
