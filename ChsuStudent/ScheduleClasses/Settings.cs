using System.ComponentModel;
using System.IO.IsolatedStorage;

namespace ChsuStudent.ScheduleClasses
{
    /// <summary>
    /// Класс настроек пользователя (автоматическая загрузка и восстановление)
    /// </summary>
    public class Settings: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private static readonly IsolatedStorageSettings AppSettings =
            IsolatedStorageSettings.ApplicationSettings;

        private const string GROUP_KEY = "ChsuStudent.Group";
        private const string TERM_KEY = "ChsuStudent.Term";

        /// <summary>
        /// Текущая группа
        /// </summary>
        private string _currentGroup;
        /// <summary>
        /// Текущий семестр
        /// </summary>
        private int _currentTerm;

        public string CurrentGroup
        {
            set
            {
                if (value == null || _currentGroup == value)
                    return;
                _currentGroup = value;

                AppSettings[GROUP_KEY] = _currentGroup;
                AppSettings.Save();
             
                NotifyPropertyChanged("CurrentGroup");
            }
            get
            {
                if (string.IsNullOrEmpty(_currentGroup))
                {
                    var value = AppSettings.Contains(GROUP_KEY) ? AppSettings[GROUP_KEY] : null;
                    _currentGroup = value != null ? value.ToString() : "1СПО-31";
                }
                return _currentGroup;
            }
        }

        public int CurrentTerm
        {
            get
            {
                if (_currentTerm == 0)
                {
                    _currentTerm = AppSettings.Contains(TERM_KEY) ? (int)AppSettings[TERM_KEY] : 1;
                }
             
                return _currentTerm;
            }
            set
            {
                if (_currentTerm == value)
                    return;
                _currentTerm = value;

                AppSettings[TERM_KEY] = _currentTerm;
                AppSettings.Save();

                NotifyPropertyChanged("CurrentTerm");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
