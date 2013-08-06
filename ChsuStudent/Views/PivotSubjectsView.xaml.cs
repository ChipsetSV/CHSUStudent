using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ChsuStudent.ScheduleClasses;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ChsuStudent.Views
{
    public partial class PivotSubjectsView : PhoneApplicationPage
    {
        /// <summary>
        /// Класс работы с расписанием
        /// </summary>
        private static Schedule _schedule;

        public PivotSubjectsView()
        {
            InitializeComponent();
            // Локализация меню
            var item = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            item.Text = ChsuStudent.Resources.Interface.MenuItemToday;
            item = (ApplicationBarMenuItem)ApplicationBar.MenuItems[1];
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

        /// <summary>
        /// Перейти в расписании на сегодняшний день
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemToday_Click(object sender, System.EventArgs e)
        {
            _schedule.ChangeDate(DateTime.Today);
        }

        /// <summary>
        /// Обновить данные расписания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemUpdate_Click(object sender, EventArgs e)
        {
            _schedule.LoadData();
        }

    }
}