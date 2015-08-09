using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace AyyMaokai //TODO: shrink images in size with TruePNG
                    //TODO: find out whether it crashes and if it does then find out why
                    //TODO: exit handler?
{
    class Program
    {
        const string logfile = "log.txt";
        const string crashesfile = "crashes.txt";
        static readonly Mutex mutex = new Mutex(true, "I will play with my quadcopter tomorrow :D Can't wait [clutterfunk]");

        static void Main()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
                return;

            try
            {
                new Thread(Log).Start();

                Thread.Sleep(60000); //wait 1 min because u don't want screenshot of starting up windows

                while (true)
                {
                    GetScreenShot().Save(DateTime.Now.Ticks + ".png", ImageFormat.Png);
                    Thread.Sleep(600000); //10 min
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(crashesfile, "[" + DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString() + "] I'VE CRASHED IN MAIN. REASON: " + e.Message + " FROM " + e.Source + "\n");
            }
            finally
            {
                File.AppendAllText(crashesfile, "[" + DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString() + "] I'VE REACHED FINALLY BLOCK IN MAIN\n");
            }
        }

        static void Log()
        {
            try
            {
                File.AppendAllText(logfile, "\n" + DateTime.Now.Ticks + "\n" + DateTime.Now.AddSeconds(1).Ticks + "\n");

                while (true)
                {
                    Thread.Sleep(60000); //1 min

                    var lines = File.ReadAllLines(logfile);
                    lines[lines.Length - 1] = DateTime.Now.Ticks.ToString();
                    File.WriteAllLines(logfile, lines);
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(crashesfile, "[" + DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString() + "] I'VE CRASHED IN LOG. REASON: " + e.Message + " FROM " + e.Source + "\n");
            }
            finally
            {
                File.AppendAllText(crashesfile, "[" + DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString() + "] I'VE REACHED FINALLY BLOCK IN LOG\n");
            }
            
        }

        static Bitmap GetScreenShot()
        {
            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            try
            {
                using (var graphics = Graphics.FromImage(bitmap))
                    graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            }
            catch (Exception e)
            {
                File.AppendAllText(crashesfile, "[" + DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString() + "] I'VE CRASHED IN GETSCREENSHOT. REASON: " + e.Message + " FROM " + e.Source + "\n");
            }

            return bitmap;
        }
    }
}