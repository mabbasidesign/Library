using System;
using System.Collections.Generic;
using LibraryData.Models;
using System.Text;

namespace LibraryServices
{
    public class DataHelpers
    {
        public static IEnumerable<string> HumanizeBizHours(IEnumerable<BranchHours> branchHours)
        {
            var hours = new List<string>();

            foreach(var time in branchHours)
            {
                var day = HumanizDay(time.DayOfWeek);
                var openTime = HumanizeTime(time.DayOfWeek);
                var closeTime = HumanizeTime(time.CloseTime);

                var timeEntery = $"{day} {openTime} to {closeTime}";
                hours.Add(timeEntery);
            }

            return hours;
        }

        private static string HumanizDay(int number)
        {
            return Enum.GetName(typeof(DayOfWeek), number);
        }

        private static string HumanizeTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh': 'mm");
        }

    }
}
