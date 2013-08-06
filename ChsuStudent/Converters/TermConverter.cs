using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ChsuStudent.Converters
{
    public class TermConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool res = false;
            if (parameter != null)
            {
                switch ((string)parameter)
                {
                    case "2":
                        if ((int)value == 2)
                            res = true;
                        break;
                    default:
                        if ((int)value == 1)
                            res = true;
                        break;
                }
            }
            else
            {
                if ((int)value == 1)
                    res = true;
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                switch ((string) parameter)
                {
                    case "2":
                        if ((bool) value)
                            return 2;
                        return 1;
                    default:
                        if ((bool) value)
                            return 1;
                        return 2;
                }
            }
            if ((bool)value)
                return 1;
            return 2;
        }
    }
}
