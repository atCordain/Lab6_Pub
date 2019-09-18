using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Lab6_Pub
{
    public class Bouncer
    {
        private int MAX_ENTRYTIME = 15;
        private int MIN_ENTRYTIME = 10;

        // TODO eventuellt Singleton.
        private Random random = new Random();
        private Patron patron;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private double speed = 1;

        public Bouncer()
        {
        }

        private string CheckID()
        {
            string[] names = { "Johan", "Tommy", "Petter", "Calle", "Kolle", "Per", "Nisse", "Frippe", "Machmud", "Jonna", "Sara" };
            return names[random.Next(names.Length)];
        }

        public Patron CreatePatron()
        {
            patron = new Patron(CheckID());
            Thread.Sleep((int)(random.Next(MAX_ENTRYTIME - MIN_ENTRYTIME) + MIN_ENTRYTIME * speed * 1000));
            return patron;
        }

        public void CancelPatrons()
        {
            cancellationTokenSource.Cancel();
        }

        public void SetSpeed(double speed)
        {
            this.speed = speed;
        }
    }
}