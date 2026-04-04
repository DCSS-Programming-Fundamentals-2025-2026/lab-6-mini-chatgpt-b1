public class TestProgram
{
    public static void Main()
    {
        string path = "TestText";
        File.WriteAllText(path, Console.ReadLine());

        string content = File.ReadAllText(path);
        Console.WriteLine(content);


    }
}