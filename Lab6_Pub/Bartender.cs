using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Bartender : Agent
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;

        public static event EventHandler LogThis;
        private bool isActive;
        private Queue<Action> actionQueue;

        private Patron patronToServe;

        private const int TimeToPourBeer = 3;
        private ConcurrentQueue<Patron> beerQueue;

        public override bool IsActive { get => isActive; set => isActive = value; }

        public Bartender(ConcurrentQueue<Patron> beerQueue)
        {
            actionQueue = new Queue<Action>();
            this.beerQueue = beerQueue;
            Initialize();
        }

        public override void Initialize()
        {
            actionQueue.Clear();
            actionQueue.Enqueue(TakeGlass);
            actionQueue.Enqueue(PourBeer);
            actionQueue.Enqueue(GiveBeer);
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
                while (!token.IsCancellationRequested && (Bar.IsBarOpen || Bar.PatronsInBar > 0))
                {
                    if (Bar.beerQueue.Count > 0)
                    {
                        action = actionQueue.Dequeue();
                        action();
                    }
                    Task.Delay(Bar.DefaultWaitTime * 1000);
                }
                if (isActive) End();
            }, token);
        }

        public override void Pause()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"Bartender took a break"));
            isActive = false;
        }

        public override void End()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"Bartender left the bar"));
            isActive = false;
        }

        public void TakeGlass()
        {
            if (Bar.AvailableGlasses > 0 && Bar.beerQueue.Count > 0)
            {
                Bar.AvailableGlasses -= 1;
                LogThis(this, new EventMessage($"Bartender goes to shelf"));
            }
            else
            {
                Thread.Sleep(Bar.DefaultWaitTime * 1000);
                Initialize();
            }
        }
        private void PourBeer()
        {
            Thread.Sleep(TimeToPourBeer * 1000);
        }

        private void GiveBeer()
        {
            beerQueue.TryDequeue(out patronToServe);
            patronToServe.GiveBeer();
            LogThis(this, new EventMessage($"Poured a beer for {patronToServe.Name}"));
        }
    }
}
