

using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace Lab6_Pub
{
    public class Waitress
    {
        const int TIME_TO_PICKUP_GLASS = 3; 
        const int TIME_TO_WASH_GLASS = 3;
        int dirtyGlasses, cleanGlasses;

        public Waitress()
        {

        }

        public void PickUpglasses(int dirtyGlasses)
        {
            Thread.Sleep(TIME_TO_PICKUP_GLASS * 1000);
            this.dirtyGlasses = dirtyGlasses;
        }

        public void WashGlases()
        {
            Thread.Sleep(TIME_TO_WASH_GLASS * 1000);
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
    }
}