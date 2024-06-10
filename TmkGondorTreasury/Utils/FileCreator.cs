using System.IO;
namespace TmkGondorTreasury.Utils;
public class FileCreator(string content)
{
    private readonly string content = content;
    private readonly string fileName = "output.txt";

    private bool FileExists()
    {
        string directory = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(directory, fileName);
        return File.Exists(filePath);
    }

    public void CreateFile()
    {
        if (FileExists())
        {
            return;
        }
        else
        {
            string directory = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(directory, fileName);

            using StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }
    }
}