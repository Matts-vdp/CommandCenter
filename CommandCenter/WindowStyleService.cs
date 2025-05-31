namespace CommandCenter;

public class WindowStyleService
{
    private readonly IntPtr _windowHandle;
    public bool IsClickThrough { get; private set; }

    public WindowStyleService(IntPtr windowHandle)
    {
        _windowHandle = windowHandle;
        IsClickThrough = false;
    }

    public void ToggleClickThrough()
    {
        SetClickThrough(!IsClickThrough);
    }

    private void SetClickThrough(bool enable)
    {
        if (_windowHandle == IntPtr.Zero) return;
        
        var extendedStyle = ClickThroughManager.GetWindowLong(_windowHandle, ClickThroughManager.GWL_EXSTYLE);
        var currentStyle = (uint)extendedStyle.ToInt64();

        var newStyle = enable
            ? currentStyle | ClickThroughManager.WS_EX_TRANSPARENT
            : currentStyle & ~ClickThroughManager.WS_EX_TRANSPARENT;
        
        ClickThroughManager.SetWindowLongPtr(_windowHandle, ClickThroughManager.GWL_EXSTYLE, new IntPtr(newStyle));

        IsClickThrough = enable;
    }
}