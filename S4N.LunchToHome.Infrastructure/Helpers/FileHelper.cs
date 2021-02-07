using System.IO;

namespace S4N.LunchToHome.Infrastructure.Helpers
{
    public class FileHelper : IFileHelper
    {
        public string[] GetContentFile(string path)
        {
            return File.ReadAllLines(path);
        }

        public void WriteContentOnFile(string path, string content, string header = null)
        {
            bool appendHeader = !File.Exists(path);

            using (var file = new System.IO.StreamWriter(path, true))
            {
                if (appendHeader)
                {
                    file.WriteLine(header);
                }

                file.WriteLine(content);
            }
        }
    }
}