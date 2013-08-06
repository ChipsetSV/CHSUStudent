using System;
using System.Windows.Navigation;
using ChsuStudent.Enums;
using ChsuStudent.ScheduleClasses;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ChsuStudent.Views
{
    public partial class ExamsView : PhoneApplicationPage
    {
        /// <summary>
        /// Класс работы с расписанием
        /// </summary>
        private static Schedule _schedule;

        public ExamsView()
        {
            InitializeComponent();
            // Локализация меню
            var item = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            item.Text = ChsuStudent.Resources.Interface.MenuItemUpdate;
        }

        /// <summary>
        /// Переход на страницу
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
            {
                InitializePageState();
            }
        }

        /// <summary>
        /// Инициализация состояния страницы
        /// </summary>
        private void InitializePageState()
        {
            if (_schedule == null)
                _schedule = new Schedule(Dispatcher);
            DataContext = _schedule;
        }

        private void menuItemUpdate_Click(object sender, EventArgs e)
        {
            _schedule.LoadData(ScheduleType.Exams);
        }
    }
}