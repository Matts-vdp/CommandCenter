﻿using System.Windows;
using System.Windows.Input;

namespace CommandCenter.Models;

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
    }
}

public class ActionHotKey : Hotkey
{
    private readonly Action _action;
    public override uint Modifier => Modifiers.MOD_CONTROL | Modifiers.MOD_ALT;

    public override Key Key { get; }

    public ActionHotKey(Key key, Action action)
    {
        _action = action;
        Key = key;
    }

    public override void Execute()
    {
        _action();
    }
}