using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PanGIF.ViewModels;
using System.Collections.Generic;

namespace PanGIF
{
    public class MainWindow : Window
    {
        private static FileDialogFilter _gifFilter = new FileDialogFilter() { Name = "GIFs", Extensions = new List<string>() { "gif" } };

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void OnOpenFile(object sender, RoutedEventArgs args)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filters.Add(_gifFilter);
            var fnames = await openFileDialog.ShowAsync(this);

            if (fnames.Length > 0)
            {
                var vm = (DataContext as MainWindowViewModel);
                await vm.ChangeSourceFile(fnames[0]);
            }
        }

        public async void OnSaveFile(object sender, RoutedEventArgs args)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filters.Add(_gifFilter);
            var fname = await saveFileDialog.ShowAsync(this);

            if (fname != string.Empty)
            {
                var vm = (DataContext as MainWindowViewModel);
                await vm.SavePanned(fname);
            }
        }

        public void OnDeleteKeyframe(object sender, RoutedEventArgs args)
        {
            (DataContext as MainWindowViewModel).DeleteCurrentCrop();
        }
    }
}
