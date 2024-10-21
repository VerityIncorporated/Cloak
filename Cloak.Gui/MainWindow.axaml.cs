using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Cloak.Gui;

public partial class MainWindow : Window
{
    private readonly Core.Cloak _cloak = new();

    private static FilePickerFileType Dll { get; } = new("DLL File")
    {
        Patterns = ["*.dll"],
        AppleUniformTypeIdentifiers = ["com.microsoft.windows-library"],
        MimeTypes = ["application/vnd.microsoft.portable-executable"]
    };
    
    public MainWindow()
    {
        InitializeComponent();
        Title = "Cloak Obfuscator";
        CanResize = false;
        Height = 400;
        Width = 350;
        
        Protections.ItemsSource = _cloak.Protections.ConvertAll(p => p.Name);
    }

    // ReSharper disable once UnusedParameter.local
    private void ProtectButtonClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputBox.Text) || string.IsNullOrWhiteSpace(OutputBox.Text)) return;
        if (Protections.SelectedItems is not null)
            _cloak.Protections.ForEach(p => p.Enabled = Protections.SelectedItems.Contains(p.Name));
        _cloak.Protect(InputBox.Text, OutputBox.Text);
    }
    
    private async void BrowseButtonClick(object sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open DLL File",
            AllowMultiple = false,
            FileTypeFilter = [Dll]
        });

        if (files.Count < 1) return;
        
        InputBox.Text = files[0].TryGetLocalPath();
        OutputBox.Text = files[0].TryGetLocalPath() + ".cloak";
    }
}