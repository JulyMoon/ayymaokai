using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
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
        //private const string crashesfile = "crashes.txt";
        private static readonly Mutex mutex = new Mutex(true, "clutterfunk");
        private static readonly TimeSpan logTimespan = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan screenshotTimespan = TimeSpan.FromMinutes(10);

        public static void Main()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
                return;

            LogStuff();

            Thread.Sleep(TimeSpan.FromMinutes(1)); // we don't want screenshot of starting up windows

            ScreenshotStuff();
        }

        private static void LogStuff()
        {
            var logTimer = new Timer(logTimespan.TotalMilliseconds);
            logTimer.Elapsed += logTimer_Elapsed;

            File.AppendAllText(logfile, "\n" + DateTime.Now.Ticks + "\n" + DateTime.Now.AddSeconds(1).Ticks + "\n");

            LogTick(); // start logging immediately
            logTimer.Start();
        }

        private static void ScreenshotStuff()
        {
            var screenshotTimer = new Timer(screenshotTimespan.TotalMilliseconds);
            screenshotTimer.Elapsed += screenshotTimer_Elapsed;

            ScreenshotTick(); // start screenshotting right off the bat
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
            GetScreenshot().Save(DateTime.Now.Ticks + ".png", ImageFormat.Png);
            Thread.Sleep(TimeSpan.FromMinutes(10));
        }

        private static void logTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogTick();
        }

        private static void screenshotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ScreenshotTick();
        }

        private static Bitmap GetScreenshot()
        {
            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(bitmap))
                graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0,
                    Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            return bitmap;
        }
    }
}