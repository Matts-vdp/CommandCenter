using System.Windows.Input;
using System.Windows.Interop;
using CommandCenter.Models;

namespace CommandCenter;

public class GlobalHotkeyService
{
    private readonly IntPtr _windowHandle;
    private readonly HwndSource _source;
    
    private int _nextHotkeyIdBase = 9000;
    
    private readonly Dictionary<int, Hotkey> _registeredHotkeysInfo = [];

    public GlobalHotkeyService(IntPtr windowHandle, HwndSource source)
    {
        _windowHandle = windowHandle;
        _source = source;
        _source.AddHook(HwndHook);
    }

    public void RegisterHotkeys(Hotkey[] hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            var id = RegisterHotkey(hotkey);
            _registeredHotkeysInfo[id] = hotkey;
        }
    }

    private int RegisterHotkey(Hotkey hotkey)
    {
        var id = _nextHotkeyIdBase++;
        var virtualKey = (uint) KeyInterop.VirtualKeyFromKey(hotkey.Key);

        if (!HotkeyManager.RegisterHotKey(_windowHandle, id, hotkey.Modifier, virtualKey))
            throw new Exception("Failed to register hotkey");
        
        return id;

    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != HotkeyManager.WM_HOTKEY) 
            return IntPtr.Zero;
        
        var hotkeyId = wParam.ToInt32();
        if (_registeredHotkeysInfo.TryGetValue(hotkeyId, out var hotkeyTuple))
        {
            hotkeyTuple.Execute();
            handled = true;
        }

        return IntPtr.Zero;
    }

    private void UnregisterAllHotkeys()
    {
        foreach (var id in _registeredHotkeysInfo.Keys)
        {
            HotkeyManager.UnregisterHotKey(_windowHandle, id);
        }

        _registeredHotkeysInfo.Clear();
    }

    public void Dispose()
    {
        _source.RemoveHook(HwndHook);
        UnregisterAllHotkeys();
    }
}