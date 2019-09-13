

using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace Lab6_Pub
{
    internal class Waitress
    {
        const int TIMEPICKGLAS = 3; 
        const int TIMETOWASH = 3;
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
                MainWindow.actualGlasses += 1;
        }

  

        public static void GoHome()
        {
            //När alla besökare har gått så går servitrisen hem.
        }


    }
}