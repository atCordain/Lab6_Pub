using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Lab6_Pub
{
    public class Bouncer
    {
        // TODO eventuellt Singleton.
        private Random random = new Random();
        private Patron patron;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
            patron = new Patron(CheckID());
            return patron;
        }

        public void CancelPatrons()
        {
            cancellationTokenSource.Cancel();
        }
    }
}