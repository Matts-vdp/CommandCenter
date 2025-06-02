using Microsoft.Web.Administration;

namespace CommandCenter;

public static class IisManager
{
    public static Dictionary<string, bool> GetStatus()
    {
        using var serverManager = new ServerManager();

        var states = serverManager.Sites.ToDictionary(
            s => s.Name, 
            s => s.State == ObjectState.Started);
        
        return states;
    }

    public static void ToggleService(string id)
    {
        using var serverManager = new ServerManager();
        
        var site = serverManager.Sites[id];

        if (site.State == ObjectState.Started)
            site.Stop();
        else
            site.Start();
        
    }
}