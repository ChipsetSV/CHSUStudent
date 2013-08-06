using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ChsuStudent
{
    public class LocalizedStrings
    {
        public LocalizedStrings() { }
        private static Resources.Interface strings = new Resources.Interface();
        public Resources.Interface Strings
        {
            get
            {
                return strings;
            }
        }
    }
}
