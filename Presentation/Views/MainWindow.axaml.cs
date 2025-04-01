using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ICGFilter.Domain.Container;
using ICGFilter.Domain.Repository;
using ICGFilter.Domain.Services;
using ICGFilter.Presentation.ViewModels;
using ReactiveUI;

namespace ICGFilter.Presentation.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private WindowContainer _container;
    private PhotoPanel _photoPanel;
    public PhotoPanel PhotoPanel => _photoPanel;
    public MainWindow(WindowContainer container)
    {
        InitializeComponent();
        _photoPanel = new(Scroll);
        _container = container;

        Loaded += (s, e) => 
        {
            var height = (int)mainGrid.RowDefinitions[2].ActualHeight;
            var width = (int)Width;
            _photoPanel.SetBitmap(width, height);
            Scroll.Content = _photoPanel;
        };

        DataContextChanged += OpenFilterDialogAsync;
    }

    private void OpenFilterDialogAsync(object? sender, EventArgs e)
    {
        this.WhenActivated(action => 
                action(ViewModel!.DialogInteraction.RegisterHandler(OpenDialog)));
    }

    private async Task OpenDialog(IInteractionContext<WindowName, Unit> context)
    {
        var window = WindowFabric.CreateWindow(context.Input);
        window.DataContext = _container.GetModel(context.Input);
        await window.ShowDialog(this);
        context.SetOutput(Unit.Default);
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