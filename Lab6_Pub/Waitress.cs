

using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace Lab6_Pub
{
    public class Waitress
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

        public void PickUpglasses()
        {
            
                Thread.Sleep(glassDelay * 1000);
        }

        public void WashGlases()
        {
            Thread.Sleep(WashDelay * 1000);

        }

        public void PutOnShelf()
        {
                MainWindow.actualGlasses += 1;

        }

  

        public static void GoHome()
        {
            //När alla besökare har gått så går servitrisen hem.
        }


    }
}