namespace CommandCenter;

public static class IisManager
{
    public static Dictionary<string, bool> GetStatus()
    {
        return new Dictionary<string, bool>
        {
            { "Foundation", true },
            { "myprotime", false }
        };
    }
}