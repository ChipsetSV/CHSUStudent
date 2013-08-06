using System;
using System.Windows.Navigation;
using System.Windows.Threading;
using ChsuStudent.ScheduleClasses;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ChsuStudent.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static Schedule _schedule;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Локализация меню
            var button = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            button.Text = ChsuStudent.Resources.Interface.MenuItemSubjects;
            button = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            button.Text = ChsuStudent.Resources.Interface.MenuItemExams;
            var item = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            item.Text = ChsuStudent.Resources.Interface.MenuItemSettings;
        }

        private void ApplicationBarMenuItemSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SettingsView.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItemSubjectsView_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/PivotSubjectsView.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItemExamsView_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ExamsView.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
            {
                InitializePageState();
            }
        }

        private void InitializePageState()
        {
            if (_schedule == null)
                _schedule = new Schedule(Dispatcher);
            DataContext = _schedule;
        }
    }
}