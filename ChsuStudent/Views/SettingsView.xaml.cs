using System;
using System.Windows;
using System.Windows.Navigation;
using ChsuStudent.Enums;
using ChsuStudent.ScheduleClasses;
using Microsoft.Phone.Controls;
using Microsoft.Unsupported;
using Phone.Controls;
using TiltEffect = Microsoft.Unsupported.TiltEffect;

namespace ChsuStudent.Views
{
    public partial class SettingsView : PhoneApplicationPage
    {
        /// <summary>
        /// Класс работы с расписанием
        /// </summary>
        private static Schedule _schedule;

        public SettingsView()
        {
            InitializeComponent();
            TiltEffect.SetIsTiltEnabled(this, true);
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
        /// Нажатие кнопки назад
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            // Обновление данных, так как поменяли группу или семестр
            _schedule.LoadData();
            _schedule.LoadData(ScheduleType.Exams);
        }


    }
}