using System.Collections;

namespace SharedLib.UnitTesting
{
    public abstract class BaseTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract IEnumerator<object[]> GetEnumerator();
    }
}
