using System;
using System.Linq.Expressions;
using ChsuStudent.Enums;

namespace ChsuStudent.ScheduleClasses
{
    /// <summary>
    /// Класс предмета
    /// </summary>
    public class Subject
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string StringTime { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public WeekType WeekType { get; set; }
        public string StringSubject { get; set; }
        public string StringTeacher { get; set; }
        public string StringLocation { get; set; }
		
		public Subject() {
			WeekType = WeekType.Every;
			DayOfWeek = DayOfWeek.Monday;
			StringTime = "";
			StartWeek = 0;
			EndWeek = 0;
			StringSubject = "";
			StringTeacher = "";
			StringLocation = "";
		}
    }
}
