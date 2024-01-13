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

namespace MusicHub.Controls
{
    /// <summary>
    /// Логика взаимодействия для SliderControl.xaml
    /// </summary>
    public partial class SliderControl : UserControl
    {
        public SliderControl()
        {
            InitializeComponent();
        }

        private void PlaySongSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel.CustomMediaElement.UpdateMediaPosition(true, (int)PlaySongSlider.Value);
        }

        private void PlaySongSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel.CustomMediaElement.UpdateMediaPosition(false, 0);
        }
    }
}
