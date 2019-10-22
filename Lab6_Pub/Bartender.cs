using System;
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

        private Queue<Action> actions;

        private Patron patronToServe;

        private const int TimeToPourBeer = 3;

        public Bartender()
        {
            actions = new Queue<Action>();
            Initialize();
        }

        public override void Initialize()
        {
            actions.Clear();
            actions.Enqueue(TakeGlass);
            actions.Enqueue(PourBeer);
            actions.Enqueue(GiveBeer);
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
                    if (Bar.beerQueue.Count > 0)
                    {
                        action = actions.Dequeue();
                        action();
                    }
                    Thread.Sleep(Bar.DefaultWaitTime * 1000);
                }
                End();
            }, token);
        }

        public override void Cancel()
        {
            LogThis(this, new EventMessage($"Bartender went home (Cancel)"));
        }

        public override void End()
        {
            LogThis(this, new EventMessage($"Bartender went home (End)"));
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
            Bar.beerQueue.TryDequeue(out patronToServe);
            patronToServe.GiveBeer();
            LogThis(this, new EventMessage($"Poured a beer for {patronToServe.Name}"));
        }
    }
}
