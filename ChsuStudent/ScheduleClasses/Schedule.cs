using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using ChsuStudent.Converters;
using ChsuStudent.Enums;
using Encoding = ChsuStudent.Utils.Encoding;

namespace ChsuStudent.ScheduleClasses
{
    /// <summary>
    /// Класс работы с расписанием
    /// </summary>
    public class Schedule : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //private static readonly AutoResetEvent allDone = new AutoResetEvent(false);
        private readonly Dispatcher _owner;
        private static readonly Object _thisLock = new Object();

        /// <summary>
        /// Массив недель
        /// </summary>
        private static readonly List<int> _weeks = new List<int>();
        /// <summary>
        /// Настройки пользвателя
        /// </summary>
        private static Settings _settingsSchedule;
        /// <summary>
        /// Расписание учебных занятий все
        /// </summary>
        private static ObservableCollection<Subject> _listSubjects;
        /// <summary>
        /// Расписание учебных занятий на неделю
        /// </summary>
        private static ObservableCollection<Subject> _listSubjectsWeek;
        /// <summary>
        /// Расписание учебных занятий на день
        /// </summary>
        private static ObservableCollection<Subject> _listSubjectsDay;
        /// <summary>
        /// Расписание экзаменов
        /// </summary>
        private static ObservableCollection<Exam> _listExams;
        private static ObservableCollection<Exam> _sortedListExams;

        private static bool _isExamsLoaded = false;
        private static bool _isSubjectsLoaded = false;

        /// <summary>
        /// Исходные данные в виде отдельных строк (расписание предеметов)
        /// </summary>
        private static List<string> _dataStrArraySubjects;
        /// <summary>
        /// Исходные данные в виде отдельных строк (расписание экзаменов)
        /// </summary>
        private static List<string> _dataStrArrayExams;
        /// <summary>
        /// Общий список групп
        /// </summary>
        private static ObservableCollection<string> _groups;

        /// <summary>
        /// Текущая неделя (получается со страницы)
        /// </summary>
        private static string _currentWeek = "0";
        /// <summary>
        /// Текущая дата (получается со страницы)
        /// </summary>
        private static string _currentDate = "00.00.0000";

        /// <summary>
        /// Текущая выбранная дата для расписания
        /// </summary>
        private static DateTime _currentSelectedDate = DateTime.Today;
        /// <summary>
        /// Номер текущей выбранной недели
        /// </summary>
        private static int _currentSelectedWeek;
        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        private static string _errorMessage = string.Empty;
        

        #region Свойства
        
        public string CurrentWeek
        {
            get
            {
                return _currentWeek;
            }
        }

        public string CurrentDate
        {
            get
            {
                return _currentDate;
            }
        }

        public ObservableCollection<string> Groups
        {
            get
            {
                return _groups;
            }
        }

        public Settings Settings
        {
            get
            {
                return _settingsSchedule;
            }
        }

        public ObservableCollection<Subject> Subjects
        {
            get
            {
                if (_listExams.Count == 0 && !_isSubjectsLoaded)
                    LoadData(ScheduleType.Subjects);
                return _listSubjects;
            }
        }

        public ObservableCollection<Subject> SubjectsWeek
        {
            get
            {
                if (_listExams.Count == 0 && !_isSubjectsLoaded)
                    LoadData(ScheduleType.Subjects);
                return _listSubjectsWeek;
            }
        }

        public ObservableCollection<Subject> SubjectsDay
        {
            get
            {
                if (_listExams.Count == 0 && !_isSubjectsLoaded)
                    LoadData(ScheduleType.Subjects);
                return _listSubjectsDay;
            }

        }

        public ObservableCollection<Exam> Exams
        {
            get
            {
                if (_listExams.Count == 0 && !_isExamsLoaded)
                    LoadData(ScheduleType.Exams);
                return _sortedListExams;
            }
        }

        public DateTime CurrentSelectedDate
        {
            get
            {
                return _currentSelectedDate;
            }
            set
            {
                ChangeDate(value);
            }
        }

        public int CurrentSelectedWeek
        {
            get
            {
                return _currentSelectedWeek;
            }
            set
            {
                ChangeWeek(value);
            }
        }

        public List<int> Weeks
        {
            get
            {
                return _weeks;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        #endregion

        

        private void NotifyPropertyChanged(string propertyName)
        {

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }


	   public Schedule(Dispatcher owner)
	   {
           if (owner == null)
           {
               throw new ArgumentNullException("owner");
           }

           _owner = owner;
        
           if (_settingsSchedule == null)
           {
               _settingsSchedule = new Settings();
               _dataStrArraySubjects = new List<string>();
               _dataStrArrayExams = new List<string>();
               _groups = new ObservableCollection<string>();
               _listSubjects = new ObservableCollection<Subject>();
               _listExams = new ObservableCollection<Exam>();
               _sortedListExams = new ObservableCollection<Exam>();

               for (int i = 0; i < 54; i++)
               {
                   _weeks.Add(i);
               }

               LoadData();
           }
	   }

       

        /// <summary>
        /// Загрузка данных в случае необходимости
        /// </summary>
        public void LoadData()
        {
            LoadData(ScheduleType.Subjects);
        }

        /// <summary>
        /// Загрузка данных в случае необходимости
        /// </summary>
        public void LoadData(ScheduleType scheduleType)
        {
            var uri = new Uri("http://rasp.chsu.ru/_student.php");

            LoadData(uri.ToString(), scheduleType);
        }

        /// <summary>
        /// Формирование запроса для отправки серверу
        /// </summary>
        /// <param name="uri">адрес страницы</param>
        /// <param name="scheduleType">тип расписания</param>
        public void LoadData(string uri, ScheduleType scheduleType)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
       
            //Add these, as we're doing a POST
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            switch (scheduleType)
            {
                case ScheduleType.Subjects:
                    request.BeginGetRequestStream(GetRequestStreamSubjectsCallback, request);
                    break;
                case ScheduleType.Exams:
                    request.BeginGetRequestStream(GetRequestStreamExamsCallback, request);
                    break;
            }
            

            // Keep the main thread from continuing while the asynchronous
            // operation completes. A real world application
            // could do something useful such as updating its user interface. 
            
            //allDone.WaitOne();
        }

        /// <summary>
        /// При открытии потока для записи отправляем параметры для получения расписания предметов
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetRequestStreamSubjectsCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            using (Stream postStream = request.EndGetRequestStream(asynchronousResult))
            {
                // Устанавливаем переменную с передаваемыми параметрами
                var contentParameters = new StringBuilder();
                contentParameters.Append("gr=").Append(_settingsSchedule.CurrentGroup);
                contentParameters.Append("&ss=").Append(_settingsSchedule.CurrentTerm);
                contentParameters.Append("&mode=").Append("Расписание занятий");

                string postData = contentParameters.ToString();

                // Convert the string into a byte array.
                byte[] byteArray = Encoding.GetEncoding(1251).GetBytes(postData);

                // Write to the request stream.
                postStream.Write(byteArray, 0, postData.Length);
            }

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(GetResponseSubjectsCallback, request);
            //allDone.WaitOne();
        }

        /// <summary>
        /// При открытии потока для записи отправляем параметры для получения расписания экзаменов
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetRequestStreamExamsCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            using (Stream postStream = request.EndGetRequestStream(asynchronousResult))
            {
                // Устанавливаем переменную с передаваемыми параметрами
                var contentParameters = new StringBuilder();
                contentParameters.Append("gr=").Append(_settingsSchedule.CurrentGroup);
                contentParameters.Append("&ss=").Append(_settingsSchedule.CurrentTerm);
                contentParameters.Append("&mode=").Append("Расписание экзаменов");

                string postData = contentParameters.ToString();

                // Convert the string into a byte array.
                byte[] byteArray = Encoding.GetEncoding(1251).GetBytes(postData);

                // Write to the request stream.
                postStream.Write(byteArray, 0, postData.Length);
            }

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(GetResponseExamsCallback, request);
        }

        /// <summary>
        /// Получаем ответ от сервера по заданным параметрам, обрабатываем данные (расписание предметов)
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetResponseSubjectsCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;
            var builder = new StringBuilder(100000);

            // End the operation
            try
            {
                using (var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult))
                {

                    using (Stream data = response.GetResponseStream())
                    {

                        using (var reader = new BinaryReader(data))
                        {
                            var temp = new byte[1024];
                            int s;
                            while ((s = reader.Read(temp, 0, 1024)) != 0)
                            {
                                builder.Append(Encoding.GetEncoding(1251).GetString(temp, 0, s));
                            }
                        }
                    }
                }
            }
            catch
            {
                _errorMessage = Resources.Schedule.CannotConnectError;
            }

            //allDone.Set();

            string dataStr = builder.ToString();

            // Разбиваем полученную строку построчно
            _dataStrArraySubjects.Clear();
            int position = 0;
            lock (_thisLock)
            {
                int res;
                while ((res = dataStr.IndexOf("\n", position, StringComparison.Ordinal)) != -1)
                {
                    _dataStrArraySubjects.Add(dataStr.Substring(position, res - position));
                    position = res + 1;
                }
            }

            // Вызываем из потока парсинг данных и обновление полей
            Action action = ParseData;
            _owner.BeginInvoke(action);

            action = ParseSubjects;
            _owner.BeginInvoke(action);

            Action<DateTime> actionChangeDate = ChangeDate;
            _owner.BeginInvoke(actionChangeDate, DateTime.Today);

            Action<string> actionErrorMessage = NotifyPropertyChanged;
            _owner.BeginInvoke(actionErrorMessage, "ErrorMessage");
        }

        /// <summary>
        /// Получаем ответ от сервера по заданным параметрам, обрабатываем данные (экзамены)
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetResponseExamsCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;
            var builder = new StringBuilder(100000);

            // End the operation
            try
            {
                using (var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult))
                {

                    using (Stream data = response.GetResponseStream())
                    {

                        using (var reader = new BinaryReader(data))
                        {
                            var temp = new byte[1024];
                            int s;
                            while ((s = reader.Read(temp, 0, 1024)) != 0)
                            {
                                builder.Append(Encoding.GetEncoding(1251).GetString(temp, 0, s));
                            }
                        }
                    }
                }
            }
            catch
            {
                _errorMessage = Resources.Schedule.CannotConnectError;
            }


            string dataStr = builder.ToString();

            // Разбиваем полученную строку построчно
            _dataStrArrayExams.Clear();
            int position = 0;
            lock (_thisLock)
            {
                int res;
                while ((res = dataStr.IndexOf("\n", position, StringComparison.Ordinal)) != -1)
                {
                    _dataStrArrayExams.Add(dataStr.Substring(position, res - position));
                    position = res + 1;
                }
            }

            // Вызываем из потока парсинг данных и обновление полей
            Action action = ParseExams;
            _owner.BeginInvoke(action);

            Action<string> actionErrorMessage = NotifyPropertyChanged;
            _owner.BeginInvoke(actionErrorMessage, "ErrorMessage");
        }

        /// <summary>
        /// Обработка полученных данных
        /// </summary>
        private void ParseData()
        {
            _groups.Clear();
            // Производим поиск необходимых элементов
            foreach (string line in _dataStrArraySubjects)
            {
                // Проверяем, нет ли элемента строки в строке документа исходного
                if (line.Contains("учебная неделя"))
                {
                    const string pattern = ">(\\d+)<";
                    if (Regex.IsMatch(line, pattern))
                    {
                        lock (_thisLock)
                        {
                            _currentWeek = Regex.Match(line, pattern).Groups[1].Value;    
                        }
                        NotifyPropertyChanged("CurrentWeek");
                    }
                }

                // Проверяем, нет ли элемента строки в строке документа исходного
                if (line.Contains("Сегодня"))
                {
                    const string pattern = "(\\d{2}.\\d{2}.\\d{4})";
                    if (Regex.IsMatch(line, pattern))
                    {
                        lock (_thisLock)
                        {
                            _currentDate = Regex.Match(line, pattern).Groups[1].Value;
                        }
                        NotifyPropertyChanged("CurrentDate");
                    }
                }

                // Составляем список групп
                if (line.Contains("option"))
                {
                    const string pattern = "value=\"(\\w+-\\w+)\"";
                    if (Regex.IsMatch(line, pattern))
                    {
                        lock (_thisLock)
                        {
                            _groups.Add(Regex.Match(line, pattern).Groups[1].Value);
                        }
                    }
                }
            }
            CheckGroup();
            NotifyPropertyChanged("Groups");
        }

        /// <summary>
        /// Проверка группы, указанной в настройках пользователя, на валидность
        /// </summary>
        private void CheckGroup()
        {
            if (!_groups.Contains(_settingsSchedule.CurrentGroup))
                _settingsSchedule.CurrentGroup = _groups.Count > 0 ? _groups[0] : string.Empty;
            if (_groups.Count == 0)
                _groups.Add(_settingsSchedule.CurrentGroup);
        }

       /// <summary>
       /// Обработка данных для получения структурированного расписания
       /// </summary>
        private void ParseSubjects()
       {
            _listSubjects.Clear();
           int i = 0;
           // Производим поиск необходимых элементов
           while (i < _dataStrArraySubjects.Count)
           {
               // Проверяем, нет ли элемента строки в строке документа исходного
               if (_dataStrArraySubjects[i].Contains("bgcolor=#ddddee>&nbsp;"))
               {
                   // Создаем объект предмета и заполняем его поля
                   var subject = new Subject();

                   string pattern = "&nbsp;(\\w+)&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                        subject.DayOfWeek = (DayOfWeek) (new DayOfWeekConverter()).ConvertBack(Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value, typeof(DayOfWeek), null, CultureInfo.CurrentCulture);
                   }

                   i++;
                   pattern = "&nbsp;(\\S+ - \\S+)&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                        subject.StringTime = Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value;
                   }

                   i++;
                   pattern = "&nbsp;([\\S\\s]*)&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                        subject.StringSubject = Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value;
                   }

                   i++;
                   pattern = "&nbsp;\\w+ (\\d+) \\w+ \\d+&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                        subject.StartWeek = int.Parse(Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value);
                   }
                   pattern = "&nbsp;\\w+ \\d+ \\w+ (\\d+)&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                        subject.EndWeek = int.Parse(Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value);
                   }

                   i++;
                   pattern = "&nbsp;(\\w+)&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                       string typeWeek = Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value;
                       if (typeWeek.Equals("чет"))
                       {
                           subject.WeekType = WeekType.Even;
                       }
                       else if (typeWeek.Equals("нечет"))
                       {
                           subject.WeekType = WeekType.Noteven;
                       }
                       else
                           subject.WeekType = WeekType.Every;
                   }

                   i++;
                   pattern = "&nbsp;([\\S\\s]*)&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                        subject.StringTeacher = Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value;
                   }

                   i++;
                   pattern = "&nbsp;([\\S\\s]*)&nbsp;";
                   if (Regex.IsMatch(_dataStrArraySubjects[i], pattern))
                   {
                        subject.StringLocation = Regex.Match(_dataStrArraySubjects[i], pattern).Groups[1].Value;
                   }

                   _listSubjects.Add(subject);
               }
               i++;
           }
           _isSubjectsLoaded = true;
           NotifyPropertyChanged("Subjects");
       }

        /// <summary>
        /// Обработка данных для получения структурированного расписания экзаменов
        /// </summary>
        private void ParseExams()
        {
            _listExams.Clear();
            int i = 0;
            // Производим поиск необходимых элементов
            while (i < _dataStrArrayExams.Count)
            {
                // Проверяем, нет ли элемента строки в строке документа исходного
                if (_dataStrArrayExams[i].Contains("bgcolor=#ddddee>&nbsp;"))
                {
                    // Создаем объект предмета и заполняем его поля
                    var subject = new Exam();

                    string pattern = "&nbsp;(\\d+\\.\\d+\\.\\d+)&nbsp;";
                    if (Regex.IsMatch(_dataStrArrayExams[i], pattern))
                    {
                        var date = Regex.Match(_dataStrArrayExams[i], pattern).Groups[1].Value;
                        subject.DateExam = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    }

                    i++;
                    pattern = "&nbsp;(\\w+)&nbsp;";
                    if (Regex.IsMatch(_dataStrArrayExams[i], pattern))
                    {
                        subject.DayOfWeek = (DayOfWeek)(new DayOfWeekConverter()).ConvertBack(Regex.Match(_dataStrArrayExams[i], pattern).Groups[1].Value, typeof(DayOfWeek), null, CultureInfo.CurrentCulture);
                    }

                    i++;
                    pattern = "&nbsp;(\\S+ - \\S+)&nbsp;";
                    if (Regex.IsMatch(_dataStrArrayExams[i], pattern))
                    {
                        subject.StringTime = Regex.Match(_dataStrArrayExams[i], pattern).Groups[1].Value;
                    }

                    i++;
                    pattern = "&nbsp;([\\S\\s]*)&nbsp;";
                    if (Regex.IsMatch(_dataStrArrayExams[i], pattern))
                    {
                        subject.StringSubject = Regex.Match(_dataStrArrayExams[i], pattern).Groups[1].Value;
                    }

                    i++;
                    pattern = "&nbsp;([\\S\\s]*)&nbsp;";
                    if (Regex.IsMatch(_dataStrArrayExams[i], pattern))
                    {
                        subject.StringTeacher = Regex.Match(_dataStrArrayExams[i], pattern).Groups[1].Value;
                    }

                    i++;
                    pattern = "&nbsp;([\\S\\s]*)&nbsp;";
                    if (Regex.IsMatch(_dataStrArrayExams[i], pattern))
                    {
                        subject.StringLocation = Regex.Match(_dataStrArrayExams[i], pattern).Groups[1].Value;
                    }

                    _listExams.Add(subject);
                }
                i++;
            }
            _sortedListExams.Clear();
            _sortedListExams = new ObservableCollection<Exam>(_listExams.OrderBy(exam => exam.DateExam));
            _isExamsLoaded = true;
            NotifyPropertyChanged("Exams");
        }

        /// <summary>
        /// Получение номера учебной недели
        /// </summary>
        /// <param name="date">дата, на которую нужно получить номер недели</param>
        /// <returns></returns>
        public int GetWeek(DateTime date)
        {
            // Задаем точку отсчета: если посленовогодняя дата, то устанавливаем 1 сентября предыдущего года
            var fromDate = new DateTime(date.Month < 9 ? date.Year - 1 : date.Year, 9, 1);
            var fromWeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(fromDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            // Определяем количество недель в году
            var lastDate = new DateTime(date.Month < 8 ? date.Year - 1 : date.Year, 12, 31);
            var lastWeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(lastDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            weekNumber = date.Month < 9 ? weekNumber + (lastWeekNumber - fromWeekNumber) : weekNumber - fromWeekNumber;
            return weekNumber;
        }


        /// <summary>
        /// Устанавливаем требуемую дату
        /// </summary>
        /// <param name="date">дата, которую нужно установить</param>
        /// <returns></returns>
        public void ChangeDate(DateTime date)
        {
            _currentSelectedDate = date;
            _currentSelectedWeek = GetWeek(_currentSelectedDate);

            _listSubjectsWeek =
                new ObservableCollection<Subject>(
                    _listSubjects.Where(
                        subj => subj.StartWeek <= _currentSelectedWeek && subj.EndWeek >= _currentSelectedWeek &&
                                (subj.WeekType == WeekType.Every ||
                                 (subj.WeekType == WeekType.Even && _currentSelectedWeek%2 == 0) ||
                                 (subj.WeekType == WeekType.Noteven && _currentSelectedWeek%2 != 0))));
            _listSubjectsDay =
                new ObservableCollection<Subject>(
                    _listSubjects.Where(
                        subj =>
                        subj.StartWeek <= _currentSelectedWeek && subj.EndWeek >= _currentSelectedWeek &&
                        subj.DayOfWeek == _currentSelectedDate.DayOfWeek &&
                        (subj.WeekType == WeekType.Every ||
                         (subj.WeekType == WeekType.Even && _currentSelectedWeek%2 == 0) ||
                         (subj.WeekType == WeekType.Noteven && _currentSelectedWeek%2 != 0))));

            NotifyPropertyChanged("CurrentSelectedDate");
            NotifyPropertyChanged("CurrentSelectedWeek");
            NotifyPropertyChanged("SubjectsDay");
            NotifyPropertyChanged("SubjectsWeek");
        }

        /// <summary>
        /// Устанавливаем требуемый номер недели
        /// </summary>
        /// <param name="week"></param>
        public void ChangeWeek(int week)
        {
            // Задаем точку отсчета: если посленовогодняя дата, то устанавливаем 1 сентября предыдущего года
            var fromDate = new DateTime(DateTime.Today.Month < 9 ? DateTime.Today.Year - 1 : DateTime.Today.Year, 9, 1);
            week = week - GetWeek(fromDate);
            var days = week * 7;
            fromDate = fromDate.AddDays(days);
            ChangeDate(fromDate);
        }
    
    }
}
