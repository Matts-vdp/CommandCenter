using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace CommandCenter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private HwndSource _source;
    private IntPtr _windowHandle;
    private const int MY_HOTKEY_ID_TOGGLE_VISIBILITY = 9000; // Example hotkey ID
    private const int MY_HOTKEY_ID_ACTION_2 = 9001;      // Another example
    private const int MY_HOTKEY_ID_TOGGLE_CLICKTHROUGH = 9002; // New hotkey
    
    private bool _isClickThrough = false;
    
    public MainWindow()
    {
        InitializeComponent();
        MouseDown += MainWindow_MouseDown;
    }
    
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var helper = new WindowInteropHelper(this);
        _windowHandle = helper.EnsureHandle();
        _source = HwndSource.FromHwnd(helper.EnsureHandle());
        _source.AddHook(HwndHook); // Hook to listen for Windows messages

        RegisterHotkeys();
    }
    
    private void SetClickThrough(bool clickThrough)
    {
        if (_windowHandle == IntPtr.Zero) return;

        IntPtr extendedStyle = ClickThroughManager.GetWindowLong(_windowHandle, ClickThroughManager.GWL_EXSTYLE);
        uint currentStyle = (uint)extendedStyle.ToInt64(); // Cast needed for 64-bit compatibility

        if (clickThrough)
        {
            ClickThroughManager.SetWindowLongPtr(_windowHandle, ClickThroughManager.GWL_EXSTYLE,
                new IntPtr(currentStyle | ClickThroughManager.WS_EX_TRANSPARENT));
        }
        else
        {
            ClickThroughManager.SetWindowLongPtr(_windowHandle, ClickThroughManager.GWL_EXSTYLE,
                new IntPtr(currentStyle & ~ClickThroughManager.WS_EX_TRANSPARENT));
        }
        _isClickThrough = clickThrough;
        System.Diagnostics.Debug.WriteLine($"Click-through set to: {_isClickThrough}");
    }
    
    private void RegisterHotkeys()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;

            // Register Ctrl+Alt+V to toggle visibility
            if (!HotkeyManager.RegisterHotKey(handle, MY_HOTKEY_ID_TOGGLE_VISIBILITY,
                HotkeyManager.Modifiers.MOD_CONTROL | HotkeyManager.Modifiers.MOD_ALT,
                (uint)KeyInterop.VirtualKeyFromKey(Key.V)))
            {
                MessageBox.Show("Failed to register hotkey Ctrl+Alt+V. It might be in use.", "Hotkey Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Register Ctrl+Shift+X for another action (example)
            if (!HotkeyManager.RegisterHotKey(handle, MY_HOTKEY_ID_ACTION_2,
                HotkeyManager.Modifiers.MOD_CONTROL | HotkeyManager.Modifiers.MOD_SHIFT,
                (uint)KeyInterop.VirtualKeyFromKey(Key.X)))
            {
                 MessageBox.Show("Failed to register hotkey Ctrl+Shift+X. It might be in use.", "Hotkey Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            // toggle click-through (e.g., Ctrl+Alt+C)
            if (!HotkeyManager.RegisterHotKey(_windowHandle, MY_HOTKEY_ID_TOGGLE_CLICKTHROUGH,
                    HotkeyManager.Modifiers.MOD_CONTROL | HotkeyManager.Modifiers.MOD_ALT,
                    (uint)KeyInterop.VirtualKeyFromKey(Key.C)))
            {
                MessageBox.Show("Failed to register hotkey Ctrl+Alt+C.", "Hotkey Error");
            }
        }

        private void UnregisterHotkeys()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HotkeyManager.UnregisterHotKey(handle, MY_HOTKEY_ID_TOGGLE_VISIBILITY);
            HotkeyManager.UnregisterHotKey(handle, MY_HOTKEY_ID_ACTION_2);
            HotkeyManager.UnregisterHotKey(_windowHandle, MY_HOTKEY_ID_TOGGLE_CLICKTHROUGH);

        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == HotkeyManager.WM_HOTKEY)
            {
                int hotkeyId = wParam.ToInt32();
                switch (hotkeyId)
                {
                    case MY_HOTKEY_ID_TOGGLE_VISIBILITY:
                        // Action for first hotkey: Toggle window visibility
                        if (Visibility == Visibility.Visible)
                        {
                            Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            Visibility = Visibility.Visible;
                        }
                        // You might want to add focus logic here if needed when it becomes visible
                        // this.Activate();
                        // FocusManager.SetFocusedElement(this, /* some focusable element */);
                        handled = true;
                        break;

                    case MY_HOTKEY_ID_ACTION_2:
                        // Action for second hotkey
                        MessageBox.Show("Ctrl+Shift+X was pressed!");
                        // Perform your other desired action here
                        handled = true;
                        break;
                    
                    case MY_HOTKEY_ID_TOGGLE_CLICKTHROUGH:
                        SetClickThrough(!_isClickThrough); // Toggle the current state
                        MessageBox.Show($"Overlay click-through is now: {(_isClickThrough ? "ON" : "OFF")}");
                        handled = true;
                        break;
                }
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_source != null) // Check if _source was initialized
            {
                _source.RemoveHook(HwndHook);
                _source.Dispose();
            }
            UnregisterHotkeys();
            base.OnClosed(e);
        }

    
    private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}