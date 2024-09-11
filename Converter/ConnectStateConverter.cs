using Khsw.Instrument.Demo.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Khsw.Instrument.Demo.Converter
{
    public class ConnectStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectStateEnum Cs = (ConnectStateEnum)value;
            BitmapImage bitimage = null;
            switch (Cs)
            {
                case ConnectStateEnum.Connect:
                    bitimage = new BitmapImage(new Uri("/Khsw.Instrument.Demo;component/Resources/Images/tick.png", UriKind.RelativeOrAbsolute));
                    break;
                case ConnectStateEnum.Disconnect:
                    bitimage = new BitmapImage(new Uri("/Khsw.Instrument.Demo;component/Resources/Images/cross.png", UriKind.RelativeOrAbsolute));
                    break;
                case ConnectStateEnum.UnKown:
                    bitimage = new BitmapImage(new Uri("/Khsw.Instrument.Demo;component/Resources/Images/help.png", UriKind.RelativeOrAbsolute));
                    break;
                default:
                    bitimage = new BitmapImage(new Uri("/Khsw.Instrument.Demo;component/Resources/Images/help.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            return bitimage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
