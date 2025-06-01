using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;

namespace CommandCenter;

public static class PowershellExecutor
{
    public static T? RunScriptFile<T>(string name, Dictionary<string, object>? parameters = null)
    {
        var results = RunScript(name, parameters);

        return (T?) results.FirstOrDefault()?.BaseObject;
    }
    
    public static void RunScriptFile(string name, Dictionary<string, object>? parameters = null)
    {
        RunScript(name, parameters);
    }

    private static Collection<PSObject> RunScript(string name, Dictionary<string, object>? parameters)
    {
        parameters ??= new Dictionary<string, object>();
        var scriptFilePath = $"C:\\Users\\Matts\\Projects\\Experiments\\CommandCenter\\{name}.ps1";

        using var ps = PowerShell.Create();
        
        ps.AddScript(File.ReadAllText(scriptFilePath));
        
        foreach (var (key, value) in parameters)
            ps.AddParameter(key, value);
        
        var results = ps.Invoke();
        return results;
    }
}