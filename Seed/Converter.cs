using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Seed
{
    //[ValueConversion(typeof(bool), typeof(string))]
    public class BgStartConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool titleInt = (bool)value;
            SolidColorBrush titleStr;
            switch (titleInt)
            {
                case true:
                    titleStr = Brushes.LightGreen;
                    break;
                case false:
                    titleStr = Brushes.Red;
                    break;
                default:
                    titleStr = Brushes.Yellow;
                    break;
            }


            return titleStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
           
        }
    }

    public class StartTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool titleInt = (bool)value;
            string titleStr;
            switch (titleInt)
            {
                case true:
                    titleStr = "开始\n测量";
                    break;
                case false:
                    titleStr = "结束\n测量";
                    break;
                default:
                    titleStr = "";
                    break;
            }
            return titleStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();           
        }
    }
}
