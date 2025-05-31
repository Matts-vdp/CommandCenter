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

    public static class Modifiers
    {
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;
        public const uint MOD_NOREPEAT = 0x4000; // Optional: To prevent auto-repeat when key is held down
    }

    // You can add common Virtual Key codes here if you like, e.g.:
    // public const uint VK_F1 = 0x70;
    // public const uint VK_D = 0x44;
}