using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

//TODO: shrink images in size with TruePNG
//TODO: find out whether it crashes and if it does then find out why
//TODO: exit handler?

namespace AyyMaokai
{
    class Program
    {
        private const string logfile = "log.txt";
        private const string screenshotFolder = "screenshots";
        private const string screenshotToolFilename = "screenshot-cmd.exe";
        private static readonly Mutex mutex = new Mutex(true, "riven");
        private static readonly TimeSpan logInterval = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan screenshotInterval = TimeSpan.FromMinutes(10);
        private static readonly Process screenshotTool = new Process
        {
            StartInfo =
                {
                    FileName = screenshotToolFilename,
                    // Arguments = $"-o \"{DateTime.Now.Ticks}.png\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
        };

        public static void Main()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                return;
            }

            if (!File.Exists(screenshotToolFilename))
            {
                throw new FileNotFoundException($"{screenshotToolFilename} not found");
            }

            LogStuff();

            Thread.Sleep(TimeSpan.FromMinutes(1)); // we don't want a screenshot of starting up windows

            ScreenshotStuff();

            while (true)
            {
                Thread.Sleep(Int32.MaxValue);
            }
        }

        private static void LogStuff()
        {
            var logTimer = new Timer(logInterval.TotalMilliseconds);
            logTimer.Elapsed += logTimer_Elapsed;

            File.AppendAllText(logfile, $"\n{DateTime.Now.Ticks}\n{DateTime.Now.AddSeconds(1).Ticks}\n");

            LogTick(); // start logging immediately
            logTimer.Start();
        }

        private static void ScreenshotStuff()
        {
            var screenshotTimer = new Timer(screenshotInterval.TotalMilliseconds);
            screenshotTimer.Elapsed += screenshotTimer_Elapsed;

            ScreenshotTick();
            screenshotTimer.Start();
        }

        private static void LogTick()
        {
            var lines = File.ReadAllLines(logfile);
            lines[lines.Length - 1] = DateTime.Now.Ticks.ToString();
            File.WriteAllLines(logfile, lines);
        }

        private static void ScreenshotTick()
        {
            var fileName = $"{DateTime.Now.Ticks}.png";
            screenshotTool.StartInfo.Arguments = $"-o \"{fileName}\"";
            screenshotTool.Start();
            screenshotTool.WaitForExit();

            if (!Directory.Exists(screenshotFolder))
                Directory.CreateDirectory(screenshotFolder);

            File.Move(fileName, Path.Combine(screenshotFolder, fileName));
        }

        private static void logTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogTick();
        }

        private static void screenshotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ScreenshotTick();
        }
    }
}