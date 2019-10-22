using System;
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

        private Queue<Action> actions;
        private Patron patronToServe;
        private int timeToPickUpDirtyGlasses = 3;
        private int timeToWashDirtyGlasses = 3;
        int dirtyGlasses, cleanGlasses;

        public Waitress()
        {
            actions = new Queue<Action>();
            Initialize();
        }

        public override void Initialize()
        {
            actions.Clear();
            actions.Enqueue(PickUpDirtyGlasses);
            actions.Enqueue(WashDirtyGlasses);
            actions.Enqueue(PutCleanGlassesOnShelf);
            actions.Enqueue(Initialize);
        }

        public override void Run()
        {
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;
            Task.Run(() =>
            {
                Action action;
                while ((Bar.IsBarOpen || !Bar.IsBarEmpty()) && !token.IsCancellationRequested)
                {
                    if (Bar.tableQueue.Count > 0 && Bar.AvailableTables > 0)
                    {
                        ShowPatronToTable();
                    }
                    else if (Bar.DirtyGlasses > 0 || dirtyGlasses > 0)
                    {
                        action = actions.Dequeue();
                        action();
                    }
                    Thread.Sleep(Bar.DefaultWaitTime * 1000);
                }
                End();
            }, token);
        }

        private void ShowPatronToTable()
        {
            Bar.tableQueue.TryDequeue(out patronToServe);
            patronToServe.GiveTable();
            Bar.AvailableTables -= 1;
            LogThis(this, new EventMessage($"Gave {patronToServe.Name} a table"));
        }

        public override void Cancel()
        {
            //throw new System.NotImplementedException();
        }

        public override void End()
        {
            //throw new System.NotImplementedException();
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