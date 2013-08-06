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
    public class DayOfWeekConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = (DayOfWeek) value;
            switch (day)
            {
                case DayOfWeek.Monday:
                    return "П\nН";
                case DayOfWeek.Tuesday:
                    return "В\nТ";
                case DayOfWeek.Wednesday:
                    return "С\nР";
                case DayOfWeek.Thursday:
                    return "Ч\nТ";
                case DayOfWeek.Friday:
                    return "П\nТ";
                case DayOfWeek.Saturday:
                    return "С\nБ";
                case DayOfWeek.Sunday:
                    return "В\nС";
                default:
                    return "**";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = (string)value;
            switch (day)
            {
                case "понедельник":
                    return DayOfWeek.Monday;
                case "вторник":
                    return DayOfWeek.Tuesday;
                case "среда":
                    return DayOfWeek.Wednesday;
                case "четверг":
                    return DayOfWeek.Thursday;
                case "пятница":
                    return DayOfWeek.Friday;
                case "суббота":
                    return DayOfWeek.Saturday;
                case "воскресенье":
                    return DayOfWeek.Sunday;
                default:
                    return DayOfWeek.Monday;
            }
        }
    }
}
