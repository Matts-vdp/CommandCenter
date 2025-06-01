using System.IO;
using System.Management.Automation;

namespace CommandCenter;

public static class PowershellExecutor
{
    public static async Task<T?> RunScriptFile<T>(string name, Dictionary<string, object>? parameters = null)
    {
        var results = await RunScript(name, parameters);

        return (T?) results.FirstOrDefault()?.BaseObject;
    }
    
    public static async Task RunScriptFile(string name, Dictionary<string, object>? parameters = null)
    {
        await RunScript(name, parameters);
    }

    private static async Task<PSDataCollection<PSObject>> RunScript(string name, Dictionary<string, object>? parameters)
    {
        parameters ??= new Dictionary<string, object>();
        var scriptFilePath = $"C:\\Users\\Matts\\Projects\\Experiments\\CommandCenter\\{name}.ps1";

        using var ps = PowerShell.Create();
        
        ps.AddScript(File.ReadAllText(scriptFilePath));
        
        foreach (var (key, value) in parameters)
            ps.AddParameter(key, value);
        
        var results = await ps.InvokeAsync();
        return results;
    }
}