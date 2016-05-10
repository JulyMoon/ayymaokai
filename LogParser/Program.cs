using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogParser
{
    class Program
    {
        static readonly long oneDayTicks = new TimeSpan(1, 0, 0, 0).Ticks;

        static void Main()
        {
            Console.Title = "Log Parser";

            string[] lines = File.ReadAllLines("log.txt");
            var dates = new List<DateTime>();

            for (int i = 0; i < lines.Length; i++)
                if (lines[i] == "")
                {
                    dates.Add(new DateTime(Int64.Parse(lines[i + 1])));
                    dates.Add(new DateTime(Int64.Parse(lines[i + 2])));
                    i += 2;
                }

            var days = new List<List<DateTime>> {new List<DateTime> {dates[0]}};
            int j = 1;
            bool open = true;
            while (j < dates.Count)
            {
                var lastDate = days[days.Count - 1][days[days.Count - 1].Count - 1];
                if (dates[j].Day == lastDate.Day)
                    days[days.Count - 1].Add(dates[j]);
                else
                    if (open)
                    {
                        days[days.Count - 1].Add(lastDate.Date.AddDays(1));

                        for (int i = 1; lastDate.Date.AddDays(i) < dates[j].Date; i++)
                            days.Add(new List<DateTime> { lastDate.Date.AddDays(i), lastDate.Date.AddDays(i + 1) });

                        days.Add(new List<DateTime> {dates[j].Date, dates[j]});
                    }
                    else
                        days.Add(new List<DateTime> {dates[j]});

                open = !open;
                j++;
            }

            var dayHours = new Dictionary<DateTime, TimeSpan>();
            foreach (var day in days)
            {
                var ts = TimeSpan.Zero;
                for (int i = 0; i < day.Count - 1; i += 2)
                    ts += day[i + 1] - day[i];
                dayHours.Add(day[0].Date, ts);
            }

            var overall = dayHours.Aggregate(TimeSpan.Zero, (current, pair) => current + pair.Value);
            string contents = $"Overall: {Readable(overall)}\nAverage: {Readable(new TimeSpan(overall.Ticks / dayHours.Count))}\n\n";

            int n = 1;
            foreach (var pair in dayHours)
            {
                contents += $"{n}) {pair.Key.ToShortDateString()} : {Readable(pair.Value)} ({String.Format("{0:0.00}%", (double)pair.Value.Ticks / oneDayTicks * 100)})\n";
                n++;
            }

            File.WriteAllText("readable log.txt", contents);
        }

        static string Readable(TimeSpan ts) =>
        ((ts.Days == 0 ? "" : $"{ts.Days} day" + (ts.Days > 1 ? "s " : " ")) +
         (ts.Hours == 0 ? "" : $"{ts.Hours} hour" + (ts.Hours > 1 ? "s " : " ")) +
         (ts.Minutes == 0 ? "" : $"{ts.Minutes} minute" + (ts.Minutes > 1 ? "s " : " ")) +
         (ts.Seconds == 0 ? "" : $"{ts.Seconds} second" + (ts.Seconds > 1 ? "s" : ""))).TrimEnd();
    }
}
