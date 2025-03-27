using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ICGFilter.Presentation.ViewModels;

namespace ICGFilter.Presentation.Views;

public partial class MainWindow : Window
{
    private PhotoPanel _photoPanel = new();
    public PhotoPanel PhotoPanel => _photoPanel;
    public MainWindow()
    {
        InitializeComponent();

        Loaded += (s, e) => 
        {
            var height = (int)mainGrid.RowDefinitions[2].ActualHeight;
            var width = (int)Width;
            _photoPanel.SetBitmap(width, height);
            scrol.Content = _photoPanel;
        };
    }

    // написать сервис под эти методы
    private async void OpenShowLoadDialog(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel is not null)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
            {
                Title = "Выберите изображение",
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            if (files.Count == 0 || files[0] is not IStorageFile file)
            {
                return;
            }

            if (DataContext is MainWindowViewModel vm)
            {
                await vm.LoadFile(file);
            }
        }
    }

    private async void ShowSaveDialog(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is not null)
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить изображение",
                FileTypeChoices = new[] 
                {
                    FilePickerFileTypes.ImagePng,
                    new FilePickerFileType("JPEG") { Patterns = new[] { "*.jpg", "*.jpeg" } }
                },
                DefaultExtension = ".png",
                ShowOverwritePrompt = true
            });

            if (file == null) 
            {
                return;
            }

            if (DataContext is MainWindowViewModel vm)
            {
                await vm.SaveFile(file);
            }
        }
    }
}