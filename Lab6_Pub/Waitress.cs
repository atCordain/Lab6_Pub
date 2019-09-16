

using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace Lab6_Pub
{
    public class Waitress
    {
        private int TIMEPICKGLAS = 3;
        private int TIMETOWASH = 3;
        private static int glassDelay;
        private static int washDelay;

   

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

        public int GlasDelay { get { return glassDelay; } internal set { glassDelay = value; } }
        public int WashDelay { get { return washDelay; } internal set { washDelay = value; } }

    }

}