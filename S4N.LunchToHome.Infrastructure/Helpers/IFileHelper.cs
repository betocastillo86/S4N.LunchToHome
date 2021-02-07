namespace S4N.LunchToHome.Infrastructure.Helpers
{
    public interface IFileHelper
    {
        void WriteContentOnFile(string path, string content, string header = null);
    }
}