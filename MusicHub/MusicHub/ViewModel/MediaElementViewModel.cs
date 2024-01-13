using MusicHub.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MusicHub.ViewModel
{
    public class MediaElementViewModel : BaseViewModel
    {
        #region Attached properties area

        private object lockObject = new object();
        private ObservableCollection<SongModel> musicList;
        private ObservableCollection<SongModel> likedMusicList;

        public string SelectedSongListName = string.Empty;
        public SongModel SelectedSong;


        // Media Element
        private MediaElement mediaElement;
        public MediaElement MediaElement
        {
            get { return mediaElement; }
            set
            {
                if (value != mediaElement)
                {
                    mediaElement = value;
                    OnPropertyChanged(nameof(MediaElement));
                    InitEvents();
                }
            }
        }

        // Volume
        private double volume;
        public double Volume
        {
            get { return volume; }
            set
            {
                if (value != volume)
                {
                    volume = value;
                    MediaElement.Volume = volume;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        // Is song playing
        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (value != isPlaying)
                {
                    isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        // Is song repeat
        private bool isSongRepeat;
        public bool IsSongRepeat
        {
            get { return isSongRepeat; }
            set
            {
                if (value != isSongRepeat)
                {
                    isSongRepeat = value;
                    OnPropertyChanged(nameof(IsSongRepeat));
                }
            }
        }

        // Is songs shuffle
        private bool isSongsShuffle;
        public bool IsSongsShuffle
        {
            get { return isSongsShuffle; }
            set
            {
                if (value != isSongsShuffle)
                {
                    isSongsShuffle = value;
                    OnPropertyChanged(nameof(IsSongsShuffle));
                }
            }
        }

        // Artist name
        private string artistName;
        public string ArtistName
        {
            get { return artistName; }
            set
            {
                if (value != artistName)
                {
                    artistName = value;
                    OnPropertyChanged(nameof(ArtistName));
                }
            }
        }

        // Song name
        private string songName;
        public string SongName
        {
            get { return songName; }
            set
            {
                if (value != songName)
                {
                    songName = value;
                    OnPropertyChanged(nameof(SongName));
                }
            }
        }

        // Song Duration Formated
        private string duration;
        public string Duration
        {
            get { return duration; }
            set
            {
                if (value != duration)
                {
                    duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        // Song Duration
        private int secondsMaximum;
        public int SecondsMaximum
        {
            get { return secondsMaximum; }
            set
            {
                if (value != secondsMaximum)
                {
                    secondsMaximum = value;
                    OnPropertyChanged(nameof(SecondsMaximum));
                }
            }
        }

        // Song Duration Progress
        private int secondsProgress;
        public int SecondsProgress
        {
            get { return secondsProgress; }
            set
            {
                if (value != secondsProgress)
                {
                    secondsProgress = value;
                    OnPropertyChanged(nameof(SecondsProgress));
                    OnPropertyChanged(nameof(CurrentDurationFormatted));
                }
            }
        }
        public string CurrentDurationFormatted => $"{SecondsProgress / 60:D1}:{SecondsProgress % 60:D2}";

        #endregion

        #region Constructor

        public MediaElementViewModel(
            ObservableCollection<SongModel> MusicList, 
            ObservableCollection<SongModel> LikedMusicList, 
            SongModel SelectedSong)
        {
            musicList = MusicList;
            likedMusicList = LikedMusicList;
            this.SelectedSong = SelectedSong;
            volume = 1;
        }

        #endregion

        #region End music event logic
        public void InitEvents()
        {
            if (MediaElement != null)
            {
                MediaElement.MediaEnded += MediaElement_MediaEnded;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            IsPlaying = false;

            SelectNextSong();
        }

        public void SelectPreviousSong()
        {
            ObservableCollection<SongModel> sourceList;

            if (SelectedSongListName.Equals("SongListItem"))
            {
                sourceList = musicList;
            }
            else
            {
                sourceList = likedMusicList;
            }

            int currentIndex = sourceList.IndexOf(SelectedSong);
            int previousIndex = (currentIndex - 1 + sourceList.Count) % sourceList.Count;

            if (previousIndex >= 0 && previousIndex < sourceList.Count)
            {
                SongModel previousSong = sourceList[previousIndex];
                SelectedSong = previousSong;
                StartSong(SelectedSong);
            }
        }

        public void SelectNextSong()
        {
            if (IsSongRepeat)
            {
                RepeatMusic();
            }
            else if (IsSongsShuffle)
            {
                ShuffleMusic();
            }               
            else
            {
                NextMusic();
            }
        }

        private void RepeatMusic()
        {
            StartSong(SelectedSong);
        }

        private void ShuffleMusic()
        {
            ObservableCollection<SongModel> sourceList;
            sourceList = SelectedSongListName.Equals("SongListItem") ? musicList : likedMusicList;
            Random random = new Random();

            int currentIndex = sourceList.IndexOf(SelectedSong);
            int nextIndex; 

            while (true)
            {                         
                nextIndex = random.Next(sourceList.Count);

                if (nextIndex != currentIndex)
                    break;
            };

            if (nextIndex >= 0)
            {
                SongModel nextSong = sourceList[nextIndex];
                SelectedSong = nextSong;
                StartSong(SelectedSong);
            }
        }

        private void NextMusic()
        {
            ObservableCollection<SongModel> sourceList;

            if (SelectedSongListName.Equals("SongListItem"))
            {
                sourceList = musicList;
            }
            else
            {
                sourceList = likedMusicList;
            }

            int currentIndex = sourceList.IndexOf(SelectedSong) + 1;
            int nextIndex = currentIndex % sourceList.Count;

            if (nextIndex >= 0 && nextIndex < sourceList.Count)
            {
                SongModel nextSong = sourceList[nextIndex];
                SelectedSong = nextSong;
                StartSong(SelectedSong);
            }
        } 

        #endregion

        #region Addition methods
        public void StartSong(SongModel Song)
        {
            if (Song != null)
            {
                MediaElement.Source = new Uri(Song.FilePath);
                SongName = Song.Title;
                ArtistName = Song.Artist;

                SecondsMaximum = GetSongDuration(Song);
                Duration = Song.Duration;

                MediaElement?.Play();
                StartTimer();
                PickSong(Song);

                IsPlaying = true;
            }
        }
        private void PickSong(SongModel Song)
        {
            foreach (var song in musicList)
            {
                song.IsPicked = false;
            }

            Song.IsPicked = true;
        }

        private int GetSongDuration(SongModel song)
        {
            string[] duration = song.Duration.Split(':');

            int minutes = Convert.ToInt32(duration[0]);
            int seconds = Convert.ToInt32(duration[1]);

            return minutes * 60 + seconds;
        }

        public DispatcherTimer Timer { get; private set; } = new DispatcherTimer();
        private void StartTimer()
        {
            if (Timer != null)
            {
                secondsProgress = 0;
                Timer.Stop();
                Timer.Tick -= Timer_Tick;
            }

            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                if (canUpdateMediaPosition)
                {
                    SecondsProgress++;
                }
            }
        }

        private bool canUpdateMediaPosition = true;

        public void UpdateMediaPosition(bool allowUpdateMediaPosition, int seconds)
        {
            lock (lockObject)
            {
                Timer.Start();

                canUpdateMediaPosition = allowUpdateMediaPosition;
                if (allowUpdateMediaPosition)
                {
                    SecondsProgress = seconds;
                    MediaElement.Position = TimeSpan.FromSeconds(SecondsProgress);
                }
            }
        }

        #endregion
    }
}
