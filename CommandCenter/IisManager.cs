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

    public static async Task ToggleService(string id)
    {
        var parameters = new Dictionary<string, object> { { "name", id } };
        await PowershellExecutor.RunScriptFile("test", parameters);
    }
}