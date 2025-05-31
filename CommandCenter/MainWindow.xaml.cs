using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using CommandCenter.Models;

namespace CommandCenter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private GlobalHotkeyService _hotkeyService;

    public MainWindow()
    {
        InitializeComponent();
        MouseDown += MainWindow_MouseDown;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var helper = new WindowInteropHelper(this);
        var windowHandle = helper.EnsureHandle();
        var source = HwndSource.FromHwnd(windowHandle);

        // Initialize services
        _hotkeyService = new GlobalHotkeyService(windowHandle, source!);

        // register global hotkeys
        _hotkeyService.RegisterHotkeys([
            new MessageKey(),
            new ToggleVisibilityKey(ToggleVisibility),
            new ClickThroughKey(windowHandle)
        ]);
    }

    private void ToggleVisibility()
    {
        Visibility = Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }


    private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _hotkeyService.Dispose();
        base.OnClosed(e);
    }
}