using System.Windows.Input;

namespace CommandCenter.Models;

public abstract class Hotkey
{
    public abstract uint Modifier { get; }
    public abstract Key Key { get; }
    public abstract void Execute();
}