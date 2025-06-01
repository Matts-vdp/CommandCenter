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

    public static void ToggleService(string id)
    {
        var parameters = new Dictionary<string, object> { { "name", id } };
        PowershellExecutor.RunScriptFile("test", parameters);
    }
}