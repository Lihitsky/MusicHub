using Id3;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using static MusicHub.MainWindow;
using MusicHub.View;
using MusicHub.ViewModel;

namespace MusicHub
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            MainViewModel viewModel = new MainViewModel();

            // Set the MediaElement property in MainViewModel
            viewModel.CustomMediaElement.MediaElement = MyMediaElement;

            DataContext = viewModel;
        }

        private void SongItem_Click(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = (ListBoxItem)sender;
            var selectedSong = (SongModel)listBoxItem.DataContext;
            if (selectedSong != null)
            {
                var viewModel = DataContext as MainViewModel;
                if (viewModel != null && viewModel.SelectSongCommand.CanExecute(selectedSong))
                {
                    viewModel.SelectSongCommand.Execute(selectedSong);
                    viewModel.CustomMediaElement.SelectedSongListName = listBoxItem.Name;
                }
            }
        }

        private void OnMainWindowDeactivated(object sender, EventArgs e)
        {
            VolumePopup.IsOpen = false;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            VolumePopup.IsOpen = false;
        }
    } 
}
