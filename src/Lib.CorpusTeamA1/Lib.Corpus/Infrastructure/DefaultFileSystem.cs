public class DefaultFileSystem : IFileSystem
{
    public string ReadAllText(string path)
    {
        if (path == null || path == " " || path == "")
        {
            throw new FileNotFoundException("Couldn't find directory");
        }
        return File.ReadAllText(path);
    }

    public bool Exists(string path)
    {
        return File.Exists(path);
    }
}