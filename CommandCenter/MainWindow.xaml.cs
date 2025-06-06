﻿using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using CommandCenter.Models;

namespace CommandCenter;

public partial class MainWindow : INotifyPropertyChanged
{
    private GlobalHotkeyService? _hotkeyService;
    public double OpacityValue { get; set; } = 1;

    private readonly Dictionary<string, Service> _services = new()
    {
        {"Foundation", new Service{ Id = "Foundation" }},
        {"Fusion", new Service{ Id = "myprotime" }}
    };

    public MainWindow()
    {
        InitializeComponent();
        MouseDown += MainWindow_MouseDown;
        DataContext = this;
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

    private async void FetchToken_OnClick(object sender, RoutedEventArgs e)
    {
        FetchToken.Background = Brushes.Coral;
        await PowershellExecutor.RunScriptFile("fetch-token");
        FetchToken.Background = Brushes.GreenYellow;
        await Task.Delay(5000);
        FetchToken.Background = Brushes.White;
    }

    private void ToggleIisService(string id)
    {
        IisManager.ToggleService(id);
        UpdateServices();
    }

    private void ToggleVisibility()
    {
        if (Visibility == Visibility.Visible)
        {
            if (OpacityValue < 0.7)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            OpacityValue = 0.3;
            OnPropertyChanged(nameof(OpacityValue));
            return;
        }

        OpacityValue = 1;
        OnPropertyChanged(nameof(OpacityValue));
        Visibility = Visibility.Visible;
    }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}