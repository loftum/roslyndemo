using System.Collections.Generic;

namespace RoslynDemo.Core.Studio
{
    public class Repository
    {
        public IList<T> GetAll<T>()
        {
            return new List<T>();
        }
    }
}