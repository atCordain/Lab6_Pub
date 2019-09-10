using System;

namespace Lab6_Pub
{
    public class Bouncer
    {
        // TODO eventuellt Singleton.
        internal Random random = new Random();
        internal Patron patron;

        public Bouncer()
        {
        }

        private string CheckID()
        {
            string[] names = { "johan", "Tommy", "Petter" };
            return names[random.Next(names.Length)];
        }

        public Patron CreatePatron()
        {
            patron = new Patron();
            patron.PatronName = CheckID();
            return patron;
        }
    }
}