using System.Runtime.InteropServices;

namespace CommandCenter;

public class ClickThroughManager
{
    // For getting and setting window extended styles
    public const int GWL_EXSTYLE = -20;
    public const uint WS_EX_TRANSPARENT = 0x00000020;
    public const uint WS_EX_LAYERED = 0x00080000; // Often used with transparency

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    // Use SetWindowLongPtr for 64-bit compatibility
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
}