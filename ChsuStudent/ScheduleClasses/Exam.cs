using System;
using System.Linq.Expressions;
using ChsuStudent.Enums;

namespace ChsuStudent.ScheduleClasses
{
    /// <summary>
    /// Класс экзамена
    /// </summary>
    public class Exam: IComparable
    {
        public DateTime DateExam { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string StringTime { get; set; }
        public string StringSubject { get; set; }
        public string StringTeacher { get; set; }
        public string StringLocation { get; set; }

        public Exam()
        {
			DayOfWeek = DayOfWeek.Monday;
			StringTime = "";
            DateExam = DateTime.MaxValue;
			StringSubject = "";
			StringTeacher = "";
			StringLocation = "";
		}

    
        public int CompareTo(object obj)
        {
            var tmp = (Exam)obj;
            return DateExam.CompareTo(tmp.DateExam);
        }
    }
}
