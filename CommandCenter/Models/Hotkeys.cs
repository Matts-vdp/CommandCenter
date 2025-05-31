using System.Windows;
using System.Windows.Input;

namespace CommandCenter.Models;

public class MessageKey : Hotkey
{
    public override uint Modifier => Modifiers.MOD_CONTROL | Modifiers.MOD_ALT;
    public override Key Key => Key.D;
    
    public override void Execute()
    {
        MessageBox.Show("Ctrl+alt+d was pressed!");
    }
}

public class ToggleVisibilityKey(Action toggleVisibility) : Hotkey
{
    public override uint Modifier => Modifiers.MOD_CONTROL | Modifiers.MOD_ALT;
    public override Key Key => Key.V;
    
    public override void Execute() => toggleVisibility();
}

public class ClickThroughKey(IntPtr windowHandle) : Hotkey
{
    private WindowStyleService _windowStyleService = new(windowHandle);
    
    public override uint Modifier => Modifiers.MOD_CONTROL | Modifiers.MOD_ALT;
    public override Key Key => Key.C;

    public override void Execute()
    {
        _windowStyleService.ToggleClickThrough();
        MessageBox.Show($"Overlay click-through is now: {(_windowStyleService.IsClickThrough ? "ON" : "OFF")}");
    }
}