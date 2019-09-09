using System;

namespace Lab6_Pub
{
    public class Bouncer
    {
        // TODO eventuellt Singleton.
        internal const int MAX_ENTRYTIME = 10;
        internal const int MIN_ENTRYTIME = 3;
        internal Random random = new Random();
        internal int timeToEntry = 0;
        internal Patron patron;
        public Bouncer()
        {
            //CheckID();
        }

        private string CheckID()
        {

                string[] names = { "johan", "Tommy", "Petter" };
                return names[random.Next(names.Length)];
        }

        public Patron CreatePatron()
        {
            if (timeToEntry <= 0)
            {
                timeToEntry = random.Next(MAX_ENTRYTIME - MIN_ENTRYTIME) + MIN_ENTRYTIME;
                patron = new Patron();
                patron.PatronName = CheckID();

                return patron;
            }
            else
            {
                timeToEntry -= 1;
                return null;
            }
        }
    }
}