using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Patron : Agent
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;
        private bool isActive;
        public static event EventHandler LogThis;
        private Queue<Action> actionQueue;
        private ConcurrentQueue<Patron> beerQueue;
        private ConcurrentQueue<Patron> tableQueue;
        private string name;
        private Random random = new Random();
        private bool hasBeer = false;
        private bool hasTable = false;
        private const int TimeToEnter = 1;
        private const int TimeToWalkToTable = 4;
        private const int MaxDrinkTime = 20;
        private const int MinDrinkTime = 10;
        private const int DefaultCheckTime = 1;

        public Patron(string name, ConcurrentQueue<Patron> beerQueue, ConcurrentQueue<Patron> tableQueue)
        {
            actionQueue = new Queue<Action>();
            this.name = name;
            this.tableQueue = tableQueue;
            this.beerQueue = beerQueue;
            Initialize();
        }

        public override void Initialize()
        {
            actionQueue.Enqueue(EnterBar);
            actionQueue.Enqueue(WaitForBeer);
            actionQueue.Enqueue(LookForTable);
            actionQueue.Enqueue(DrinkBeer);
            actionQueue.Enqueue(LeaveBar);
        }

        public override void Run()
        {
            isActive = true;
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;

            Action action;

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested && actionQueue.Count > 0)
                {
                    action = actionQueue.Dequeue();
                    action();
                }
                if (isActive) End();
            });

        }

        public override void Pause()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"{Name} took a break"));
            isActive = false;
        }

        public override void End()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"{Name} left the bar"));
            isActive = false;
        }        

        public void EnterBar()
        {
            Bar.PatronsInBar += 1;
            Thread.Sleep(TimeToEnter * 1000);
        }

        public void WaitForBeer()
        {
            beerQueue.Enqueue(this);
            while (!token.IsCancellationRequested)
            {
                if (hasBeer)
                {
                    LogThis(this, new EventMessage($"{Name} got a beer"));
                    break;
                }
                Thread.Sleep(DefaultCheckTime * 1000);
            }
        }

        public void LookForTable()
        {
            tableQueue.Enqueue(this);
            while (!token.IsCancellationRequested)
            {
                if (hasTable)
                {
                    Thread.Sleep(TimeToWalkToTable * 1000);
                    LogThis(this, new EventMessage($"{Name} got a table"));
                    break;
                }
                Thread.Sleep(DefaultCheckTime * 1000);
            }
        }


        public void DrinkBeer()
        {
            // slumpa 10-20 sec.
            var beerDrinkTime = (random.Next(MaxDrinkTime - MinDrinkTime) + MinDrinkTime) * 1000;
            Thread.Sleep(beerDrinkTime);
            LogThis(this, new EventMessage($"{Name} downed a beer ({beerDrinkTime/1000}s)"));

        }
        private void LeaveBar()
        {
            Bar.DirtyGlasses++;
            Bar.PatronsInBar--;
            Bar.AvailableTables++;
        }

        public string Name { get => name; set => name = value; }
        public override bool IsActive { get => isActive; set => isActive = value; }

        public void GiveBeer()
        {
            hasBeer = true;
        }

        public void GiveTable()
        {
            hasTable = true;
        }
    }
}