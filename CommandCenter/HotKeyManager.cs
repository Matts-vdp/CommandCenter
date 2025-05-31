using System.Runtime.InteropServices;

namespace CommandCenter;

public class HotkeyManager
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public const int WM_HOTKEY = 0x0312;

    // You can add common Virtual Key codes here if you like, e.g.:
    // public const uint VK_F1 = 0x70;
    // public const uint VK_D = 0x44;
}