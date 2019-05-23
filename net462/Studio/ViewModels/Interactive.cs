using System.Collections.Generic;

namespace Studio.ViewModels
{
    public class Repository
    {
        public IList<T> GetAll<T>()
        {
            return new List<T>();
        }
    }

    public class Interactive
    {
        public Repository Repo { get; } = new Repository();

        public string GetHelp()
        {
            return "help";
        }
    }
}