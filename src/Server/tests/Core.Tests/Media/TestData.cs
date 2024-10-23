namespace Media.Tests;

public class TestData
{
    public static class Images
    {
        public static Stream HeicIphone15 => GetData("001.HEIC");
        public static Stream JpgIphone15 => GetData("001.JPG");

        public static Stream FriendsJpg => GetData("002_Friends.jpg");
    }


    private static Stream GetData(string name)
    {
        var stream = new FileStream($"{GetTestDataPath()}/{name}", FileMode.Open);

        return stream;
    }

    private static string GetTestDataPath()
    {
        string[] segments = typeof(TestData).Assembly.Location.Split(Path.DirectorySeparatorChar);
        var idx = Array.IndexOf(segments, "anyding");
        var root = string.Join(Path.DirectorySeparatorChar, segments.Take(idx + 1));

        return Path.Join(root, "tests", "test-data");
    }

    public static string ResultPath => Path.Join(GetTestDataPath(), "results");
}
