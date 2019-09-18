
using System.Threading;

namespace Lab6_Pub
{
    public class Waitress
    {
        private int TIME_TO_PICKUP_GLASS = 3;
        private int TIME_TO_WASH_GLASS = 3;
        int dirtyGlasses, cleanGlasses;
        private double speed = 1;

        public Waitress()
        {

        }

        public void PickUpglasses(ref int dirtyGlasses)
        {
            Thread.Sleep((int)(TIME_TO_PICKUP_GLASS * speed * 1000));
            this.dirtyGlasses = dirtyGlasses;

            dirtyGlasses = 0; 
        }

        public void WashGlases()
        {
            Thread.Sleep((int)(TIME_TO_WASH_GLASS * speed * 1000));
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
        public void SetSpeed(double speed)
        {
            this.speed = speed;
        }
    }
}