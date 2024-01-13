using MusicHub.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicHub
{
    /// <summary>
    /// Логика взаимодействия для VolumeSliderControl.xaml
    /// </summary>
    public partial class VolumeSliderControl : UserControl
    {
        public VolumeSliderControl()
        {
            InitializeComponent();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel.CustomMediaElement.Volume = VolumeSlider.Value;
        }
    }
}
