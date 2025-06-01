using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using CommandCenter.Models;

namespace CommandCenter;

public partial class MainWindow
{
    private GlobalHotkeyService? _hotkeyService;

    private readonly Dictionary<string, Service> _services = new()
    {
        {"Foundation", new Service{ Id = "Foundation" }},
        {"Fusion", new Service{ Id = "myprotime" }}
    };

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
        List<Hotkey> hotkeys = [
            new MessageKey(),
            new ToggleVisibilityKey(ToggleVisibility),
            new ClickThroughKey(windowHandle),
        ];

        // service hotkeys
        var serviceKeys = _services.Select((service, index) =>
        {
            var key = Key.D1 + index;
            return new ActionHotKey(key, () => ToggleIisService(service.Value.Id));
        });

        hotkeys = hotkeys.Concat(serviceKeys).ToList();
        
        _hotkeyService.RegisterHotkeys(hotkeys.ToArray());
        
        UpdateServices();
    }

    private void UpdateServices()
    {
        // Get service status and set correctly
        var status = IisManager.GetStatus();
        
        foreach (var (key, value) in _services)
        {
            var button = (Button?) GetType().GetField(key, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
            button!.Background = GetColor(status[value.Id]);
        }
    }
    
    private SolidColorBrush GetColor(bool status) => status ? Brushes.Chartreuse : Brushes.Salmon;
    private void Service_OnClick(object sender, RoutedEventArgs e) => ToggleIisService(((Button) sender).Name);

    private void ToggleIisService(string id)
    {
        var parameters = new Dictionary<string, object> { { "name", id } };
        PowershellExecutor.RunScriptFile("test", parameters);
        UpdateServices();
    }

    private void ToggleVisibility() => Visibility = Visibility == Visibility.Visible 
        ? Visibility.Collapsed 
        : Visibility.Visible;

    private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    protected override void OnClosed(EventArgs e)
    {
        _hotkeyService?.Dispose();
        base.OnClosed(e);
    }
}