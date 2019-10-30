using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Waitress : Agent
    {
        public static event EventHandler LogThis;
        public override bool IsActive { get => isActive; set => isActive = value; }
        public override float SimulationSpeed { get; set; }

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;

        private int timeToPickUpDirtyGlasses = 3;
        private int timeToWashDirtyGlasses = 3;
        
        private int dirtyGlasses, cleanGlasses;
        
        private bool isActive;
        
        private Queue<Action> actionQueue;
        private Patron patronToServe;
        private Bar bar;
        
        public Waitress(Bar bar)
        {
            SimulationSpeed = 1;
            this.bar = bar;
            actionQueue = new Queue<Action>();
            Initialize();
        }

        public override void Initialize()
        {
            actionQueue.Clear();
            actionQueue.Enqueue(PickUpDirtyGlasses);
            actionQueue.Enqueue(WashDirtyGlasses);
            actionQueue.Enqueue(PutCleanGlassesOnShelf);
            actionQueue.Enqueue(Initialize);
        }

        public override void Run()
        {
            isActive = true;
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;
            Task.Run(() =>
            {
                Action action;
                while ((bar.IsBarOpen || bar.PatronsInBar > 0 || bar.AvailableGlasses < bar.TotalGlassesInBar) && !token.IsCancellationRequested)
                {
                    if (bar.GetNumberOfPatronsInTableQueue() > 0 && bar.AvailableTables > 0)
                    {
                        ShowPatronToTable();
                    }
                    else if (bar.DirtyGlasses > 0 || dirtyGlasses > 0)
                    {
                        action = actionQueue.Dequeue();
                        action();
                    }
                    Thread.Sleep((int)(Bar.DefaultWaitTime * SimulationSpeed * 1000));
                }
                if (isActive) End();
            }, token);
        }

        public override void Pause()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"Waitress took a break"));
            isActive = false;
        }

        public override void End()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"Waitress left the bar"));
            isActive = false;
        }
        private void ShowPatronToTable()
        {
            patronToServe = bar.GetPatronToServeTable();
            patronToServe.GiveTable();
            bar.AvailableTables -= 1;
            LogThis(this, new EventMessage($"Gave {patronToServe.Name} a table"));
        }

        private void PickUpDirtyGlasses()
        {
            Thread.Sleep((int)(timeToPickUpDirtyGlasses * SimulationSpeed * 1000));
            dirtyGlasses = bar.DirtyGlasses;
            bar.DirtyGlasses = 0;
            LogThis(this, new EventMessage($"Picked up {dirtyGlasses} dirty glasses"));
        }
        private void WashDirtyGlasses()
        {
            Thread.Sleep((int)(timeToWashDirtyGlasses * SimulationSpeed * 1000));
            cleanGlasses = dirtyGlasses;
            LogThis(this, new EventMessage($"Washed the glasses"));
        }
        private void PutCleanGlassesOnShelf()
        {
            bar.AvailableGlasses += cleanGlasses;
            dirtyGlasses = 0;
            LogThis(this, new EventMessage($"Put the clean glasses on shelf"));
        }
    }
}