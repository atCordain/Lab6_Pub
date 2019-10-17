using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    class Bar
    {
        // Initial values
        private const int totalGlassesInBar = 8;
        private const int totalTablesInBar = 9;
        public int TotalGlassesInBar => totalGlassesInBar;
        public int TotalTablesInBar => totalTablesInBar;

        // fields
        private int availableGlasses;
        private int availableTables;
        private int patronsInBar;
        private bool isBarOpen;
        public int AvailableGlasses => availableGlasses; 
        public int AvailableTables => availableTables; 
        public int PatronsInBar => patronsInBar;
        public bool IsBarOpen => isBarOpen;

        // Agents
        private Bartender bartender;
        private Waitress waitress;
        private Bouncer bouncer;
        private BlockingCollection<Patron> patrons;

        // Task management
        CancellationTokenSource bartenderCancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource waitressCancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource bouncerCancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource patronCancellationTokenSource = new CancellationTokenSource();


        public Bar()
        {
            bartender = new Bartender();
            waitress = new Waitress();
            bouncer = new Bouncer();
            availableGlasses = totalGlassesInBar;
            availableTables = totalTablesInBar;
            isBarOpen = false;
        }

        public void Run()
        {
            isBarOpen = true;
            StartBartender(bartenderCancellationTokenSource.Token);
            StartWaitress(waitressCancellationTokenSource.Token);
            StartBouncer(bouncerCancellationTokenSource.Token);
        }

        private void StartBartender(CancellationToken token)
        {
            Task.Run(() => 
            { 
                
            
            }, token);
        }

        private void StartWaitress(CancellationToken token)
        {
            Task.Run(() => 
            { 
            
            }, token);
            
        }

        private void StartBouncer(CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested && IsBarOpen)
                {
                    CreateAndStartPatrons();
                    Thread.Sleep(bouncer.GetSleepTime());
                }
            }, token);
        }

        private void CreateAndStartPatrons()
        {
            foreach (var patron in bouncer.CreatePatron(1))
            {
                patrons.Add(patron);
                patronsInBar = patrons.Count;
                StartPatron(patron, patronCancellationTokenSource.Token);
            }
        }

        private void StartPatron(Patron patron, CancellationToken token)
        {
            Task.Run(() =>
            {

            }, token);
        }
    }
}