using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.View
{
    public class SongModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; }
        public string FilePath { get; set; }

        private int orderId;
        public int OrderId
        {
            get { return orderId; }
            set
            {
                if (value != orderId)
                {
                    orderId = value;
                    OnPropertyChanged(nameof(OrderId));
                }
            }
        }

        private bool isPicked;
        public bool IsPicked
        {
            get { return isPicked; }
            set
            {
                if (value != isPicked)
                {
                    isPicked = value;
                    OnPropertyChanged(nameof(IsPicked));
                }
            }
        }

        private bool isLiked;
        public bool IsLiked
        {
            get { return isLiked; }
            set
            {
                if (value != isLiked)
                {
                    isLiked = value;
                    OnPropertyChanged(nameof(IsLiked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
