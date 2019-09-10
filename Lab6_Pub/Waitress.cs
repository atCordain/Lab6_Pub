

using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace Lab6_Pub
{
    internal class Waitress
    {
        const int TIMEPICKGLAS = 10; 
        const int TIMETOWASH = 15;
        public static int glassDelay;
        public static int WashDelay;

        public Waitress()
        {
            glassDelay = TIMEPICKGLAS;
            WashDelay = TIMETOWASH;
        }

        internal static void PickUpglasses()
        {
                Thread.Sleep(glassDelay * 1000);
        }

        internal static void WashGlases()
        {
            Thread.Sleep(WashDelay * 1000);

        }

        internal static void PutOnShelf()
        {
                MainWindow.actualGlasses++;
        }

  

        public static void GoHome()
        {
            //När alla besökare har gått så går servitrisen hem.
        }


    }
}