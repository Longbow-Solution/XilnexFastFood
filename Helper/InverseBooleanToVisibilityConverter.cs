using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace LFFSSK.Helper
{
    public class InverseBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is bool isFixed && values[1] is bool isReward)
            {
                return (isFixed || isReward) ? Visibility.Hidden : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is bool boolean)
        //    {
        //        return boolean ? Visibility.Collapsed : Visibility.Visible;
        //    }
        //    return Visibility.Visible;
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is Visibility visibility)
        //    {
        //        return visibility != Visibility.Visible;
        //    }
        //    return false;
        //}
    }
}
