using MusicHub.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Input;
using Id3;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace MusicHub.ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public void Execute(object parameter) => execute(parameter);
    }

    public class MainViewModel : BaseViewModel
    {
        #region Attached properties area

        private MediaElementViewModel customMediaElement;
        public MediaElementViewModel CustomMediaElement
        {
            get { return customMediaElement; }
            set
            {
                if (customMediaElement != value)
                {
                    customMediaElement = value;
                    OnPropertyChanged(nameof(CustomMediaElement));
                }
            }
        }

        // Collection
        private ObservableCollection<SongModel> musicList;
        public ObservableCollection<SongModel> MusicList
        {
            get { return musicList; }
            set
            {
                if (value != musicList)
                {
                    musicList = value;
                    OnPropertyChanged(nameof(MusicList));
                }
            }
        }

        // Liked Collection
        private ObservableCollection<SongModel> likedMusicList;
        public ObservableCollection<SongModel> LikedMusicList
        {
            get { return likedMusicList; }
            set
            {
                if (value != likedMusicList)
                {
                    likedMusicList = value;

                    OnPropertyChanged(nameof(LikedMusicList));
                }
            }
        }

        // Selected Song
        private SongModel selectedSong;
        public SongModel SelectedSong
        {
            get { return selectedSong; }
            set
            {
                if (value != selectedSong)
                {
                    selectedSong = value;
                    OnPropertyChanged(nameof(SelectedSong));
                    CustomMediaElement.StartSong(selectedSong);
                    CustomMediaElement.SelectedSong = selectedSong;
                }
            }
        }

        public bool ShowNoLikedMusicMessage
        {
            get { return LikedMusicList.Count == 0; }
        }

        public bool ShowNoMusicMessage
        {
            get { return MusicList.Count == 0; }
        }

        #endregion

        #region Commands
        public ICommand AddSongCommand { get; private set; }
        public ICommand AddManySongsCommand { get; private set; }
        public ICommand DeleteSongCommand { get; private set; }
        public ICommand LikeSongCommand { get; private set; }
        public ICommand SelectSongCommand { get; private set; }
        public ICommand PauseSongCommand { get; private set; }
        public ICommand RepeatSongCommand { get; private set; }
        public ICommand ShuffleSongCommand { get; private set; }
        public ICommand SkipSongCommand { get; private set; }
        public ICommand SkipBackwardSongCommand { get; private set; }
        public ICommand VolumeButtonPressCommand {  get; private set; }

        #endregion

        // Constructor
        public MainViewModel()
        {
            MusicList = new ObservableCollection<SongModel>();
            LikedMusicList = new ObservableCollection<SongModel>();
            CustomMediaElement = new MediaElementViewModel(MusicList, LikedMusicList, SelectedSong);
            AddSongCommand = new RelayCommand(ExecuteAddSongCommand);
            AddManySongsCommand = new RelayCommand(ExecuteAddManySongsCommand);
            DeleteSongCommand = new RelayCommand(DeleteSongCommandCommand);
            LikeSongCommand = new RelayCommand(ExecuteLikeSongCommand);
            SelectSongCommand = new RelayCommand(ExecuteSelectSongCommand);
            PauseSongCommand = new RelayCommand(ExecutePauseSongCommand);
            RepeatSongCommand = new RelayCommand(ExecuteRepeatSongCommand);
            ShuffleSongCommand = new RelayCommand(ExecuteShuffleSongCommand);
            SkipSongCommand = new RelayCommand(ExecuteSkipSongCommand);
            SkipBackwardSongCommand = new RelayCommand(ExecuteSkipBackwardSongCommand);
            VolumeButtonPressCommand = new RelayCommand(ExecuteVolumeButtonPressCommand);
        }

        #region Execute relay commands area
        private void ExecuteAddSongCommand(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 Files (*.mp3)|*.mp3|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                using (var mp3 = new Mp3(selectedFilePath))
                {
                    var tag = mp3.GetTag(Id3TagFamily.Version2X);

                    SongModel song = new SongModel()
                    {
                        Id = MusicList.Count + 1,
                        OrderId = MusicList.Count + 1,
                        Title = tag.Title,
                        Artist = tag.Artists,
                        Album = tag.Album,
                        Genre = tag.Genre,
                        Duration = $"{mp3.Audio.Duration.Minutes}:{mp3.Audio.Duration.Seconds}",
                        FilePath = selectedFilePath,
                        IsLiked = false
                    };

                    MusicList.Add(song);
                    OnPropertyChanged(nameof(ShowNoMusicMessage));
                }
            }
        }

        private void ExecuteAddManySongsCommand(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 Files (*.mp3)|*.mp3|All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string selectedFilePath in openFileDialog.FileNames)
                {
                    using (var mp3 = new Mp3(selectedFilePath))
                    {
                        var tag = mp3.GetTag(Id3TagFamily.Version2X);

                        SongModel song = new SongModel()
                        {
                            Id = MusicList.Count + 1,
                            OrderId = MusicList.Count + 1,
                            Title = tag.Title,
                            Artist = tag.Artists,
                            Album = tag.Album,
                            Genre = tag.Genre,
                            Duration = $"{mp3.Audio.Duration.Minutes}:{mp3.Audio.Duration.Seconds}",
                            FilePath = selectedFilePath,
                            IsLiked = false
                        };

                        MusicList.Add(song);
                        OnPropertyChanged(nameof(ShowNoMusicMessage));
                    }
                }
            }
        }

        private void DeleteSongCommandCommand(object parameter)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the music?", "Delete", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                MusicList.Remove(SelectedSong);
                ReorderList(MusicList);
                CustomMediaElement.MediaElement.Stop();
                SelectedSong = null;
            }
        }

        private void ExecuteLikeSongCommand(object parameter)
        {
            if (parameter is SongModel selectedSong)
            {
                if (selectedSong.IsLiked)
                {
                    LikedMusicList.Remove(selectedSong);
                    ReorderList(LikedMusicList);
                    OnPropertyChanged(nameof(ShowNoLikedMusicMessage));
                }
                else
                {
                    selectedSong.OrderId = likedMusicList.Count + 1;
                    LikedMusicList.Add(selectedSong);
                    OnPropertyChanged(nameof(ShowNoLikedMusicMessage));
                }
                selectedSong.IsLiked = !selectedSong.IsLiked;
            }
        }

        private void ExecuteSelectSongCommand(object parameter)
        {
            if (parameter is SongModel selectedSong)
            {
                SelectedSong = selectedSong;
            }
        }

        private void ExecutePauseSongCommand(object parameter)
        {
            if (CustomMediaElement.IsPlaying)
            {
                CustomMediaElement.MediaElement.Pause();
                CustomMediaElement.IsPlaying = false;
                CustomMediaElement.Timer.Stop();
            }
            else
            {
                CustomMediaElement.MediaElement.Play();
                CustomMediaElement.IsPlaying = true;
                CustomMediaElement.Timer.Start();
            }

        }
      
        private void ExecuteRepeatSongCommand(object parameter)
        {
            if (CustomMediaElement.IsSongRepeat)
            {
                CustomMediaElement.IsSongRepeat = false;
            }
            else
            {
                CustomMediaElement.IsSongRepeat = true;
                CustomMediaElement.IsSongsShuffle = false;
            }
        }
        
        private void ExecuteShuffleSongCommand(object parameter)
        {
            if (CustomMediaElement.IsSongsShuffle)
            {
                CustomMediaElement.IsSongsShuffle = false;
            }
            else
            {
                CustomMediaElement.IsSongsShuffle = true;
                CustomMediaElement.IsSongRepeat = false;
            }
        }

        private void ExecuteSkipSongCommand(object parameter)
        {
            CustomMediaElement.SelectNextSong();
        }

        private void ExecuteSkipBackwardSongCommand(object parameter)
        {
            if (CustomMediaElement.MediaElement.Position.TotalSeconds < 5)
            {
                CustomMediaElement.SelectPreviousSong();
            }
            else
            {
                CustomMediaElement.StartSong(SelectedSong);
            }
        }
       
        private void ExecuteVolumeButtonPressCommand(object parameter)
        {
            if (parameter is Popup popup)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }
        #endregion

        #region Addition methods

        private void ReorderList(ObservableCollection<SongModel> list)
        {
            foreach (var music in list)
            {
                int index = list.IndexOf(music);
                list[index].OrderId = index + 1;
            }
        }

        #endregion

    }
}
