

using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace Lab6_Pub
{
    public class Waitress
    {
        public static int TIMEPICKGLAS = 3;
        public static int TIMETOWASH = 3;
        public static int glassDelay { get; set; }
        public static int washDelay  {get; set;}

        public Waitress()
        {
            glassDelay = TIMEPICKGLAS;
            washDelay = TIMETOWASH;
        }

        public void PickUpglasses()
        {
            
                Thread.Sleep(glassDelay * 1000);
        }

        public void WashGlases()
        {
            Thread.Sleep(washDelay * 1000);

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