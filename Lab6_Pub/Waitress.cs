

using System;
using System.Timers;

namespace Lab6_Pub
{
    internal class Waitress
    {
        const int TIMEPICKGLAS = 10; 
        const int TIMETOWASH = 15;
        public static int glassDelay;
        public static int WashDelay;
        public static Timer timer = new Timer(1000d);

        public Waitress()
        {
            timer.Elapsed += PickUpglasses;
            timer.Elapsed += WashGlases;
            glassDelay = TIMEPICKGLAS;
            WashDelay = TIMETOWASH;
        }

        public static void PickUpglasses(object sender, ElapsedEventArgs e)
        { 
                glassDelay -= 1;
        }

        public static void WashGlases(object sender, ElapsedEventArgs e)
        {
            WashDelay -= 1;
        }

        public static void PutOnShelf(object sender, ElapsedEventArgs e)
        {
            while (MainWindow.actualGlasses < MainWindow.MAX_GLASSES && MainWindow.openTime != 0)
            { 
                MainWindow.actualGlasses++;
            }
        }

        public static void StopWaitress()
        {
           MainWindow.open = false;
            timer.Stop(); 
        }

        internal static void StartWaitress()
        {
            MainWindow.open = false;
            timer.Start();
        }

        public static void GoHome()
        {
            //När alla besökare har gått så går servitrisen hem.
        }

    }
}