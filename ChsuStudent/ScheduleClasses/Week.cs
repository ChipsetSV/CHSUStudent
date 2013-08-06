using System;
using System.Linq.Expressions;
using ChsuStudent.Enums;

namespace ChsuStudent.ScheduleClasses
{
    /// <summary>
    /// Класс номера недели
    /// </summary>
    public class Week
    {
        public int Number;

        public Week(int number)
        {
            Number = number;
        }
    }
}
