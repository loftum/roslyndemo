namespace RoslynDemo.Core.Studio
{
    public class Interactive
    {
        public Repository Repo { get; } = new Repository();

        public string GetHelp()
        {
            return "help";
        }
    }
}