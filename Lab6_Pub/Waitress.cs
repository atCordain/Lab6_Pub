

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
        int dirtyGlasses, cleanGlasses;
      
        public Waitress()
        {
            glassDelay = TIMEPICKGLAS;
            washDelay = TIMETOWASH;
        }

        public void PickUpglasses(int dirtyGlasses)
        {
            Thread.Sleep(TIME_TO_PICKUP_GLASS * 1000);
            this.dirtyGlasses = dirtyGlasses;
        }

        public void WashGlases()
        {
            Thread.Sleep(washDelay * 1000);
            cleanGlasses = dirtyGlasses;
        }

        public int PutOnShelf()
        {
            return cleanGlasses;
        }
      
        public string Leave()
        {
            return "Waitress has gone home";
        }
      
        public int GlasDelay { get { return glassDelay; } internal set { glassDelay = value; } }
        public int WashDelay { get { return washDelay; } internal set { washDelay = value; } }


    }

}