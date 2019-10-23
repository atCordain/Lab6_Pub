using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Waitress : Agent
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;

        public static event EventHandler LogThis;

        private bool isActive;
        private Queue<Action> actionQueue;
        private Patron patronToServe;
        private int timeToPickUpDirtyGlasses = 3;
        private int timeToWashDirtyGlasses = 3;
        int dirtyGlasses, cleanGlasses;
        private ConcurrentQueue<Patron> tableQueue;

        public override bool IsActive { get => isActive; set => isActive = value; }

        public Waitress(ConcurrentQueue<Patron> tableQueue)
        {
            this.tableQueue = tableQueue;
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
                while ((Bar.IsBarOpen || Bar.PatronsInBar > 0 || Bar.AvailableGlasses < Bar.TotalGlassesInBar) && !token.IsCancellationRequested)
                {
                    if (Bar.tableQueue.Count > 0 && Bar.AvailableTables > 0)
                    {
                        ShowPatronToTable();
                    }
                    else if (Bar.DirtyGlasses > 0 || dirtyGlasses > 0)
                    {
                        action = actionQueue.Dequeue();
                        action();
                    }
                    Thread.Sleep(Bar.DefaultWaitTime * 1000);
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
            tableQueue.TryDequeue(out patronToServe);
            patronToServe.GiveTable();
            Bar.AvailableTables -= 1;
            LogThis(this, new EventMessage($"Gave {patronToServe.Name} a table"));
        }

        private void PickUpDirtyGlasses()
        {
            Thread.Sleep((int)(timeToPickUpDirtyGlasses * 1000));
            dirtyGlasses = Bar.DirtyGlasses;
            Bar.DirtyGlasses = 0;
            LogThis(this, new EventMessage($"Picked up {dirtyGlasses} dirty glasses"));
        }
        private void WashDirtyGlasses()
        {
            Thread.Sleep((int)(timeToWashDirtyGlasses * 1000));
            cleanGlasses = dirtyGlasses;
            LogThis(this, new EventMessage($"Washed the glasses"));
        }
        private void PutCleanGlassesOnShelf()
        {
            Bar.AvailableGlasses += cleanGlasses;
            dirtyGlasses = 0;
            LogThis(this, new EventMessage($"Put the clean glasses on shelf"));
        }
    }
}