using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MusicHub
{
    internal class VolumeValueToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double volume)
            {
                if (volume > 0.7)
                {
                    return "VolumeHigh";
                }
                else if (volume > 0.4)
                {
                    return "VolumeMedium";
                }
                else if (volume > 0.05)
                {
                    return "VolumeLow";
                }
                else
                {
                    return "VolumeMute";
                }
            }
            return "VolumeLow";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
