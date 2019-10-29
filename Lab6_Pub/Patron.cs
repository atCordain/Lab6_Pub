using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Patron : Agent
    {
        public static event EventHandler LogThis;
        public string Name { get => name; set => name = value; }
        public override bool IsActive { get => isActive; set => isActive = value; }
        public override float SimulationSpeed { get; set; }
        public static int PatronSpeed = 1;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;
        
        private Queue<Action> actionQueue;
        private ConcurrentQueue<Patron> beerQueue;
        private ConcurrentQueue<Patron> tableQueue;
        private Random random = new Random();
        
        private string name;
        private bool hasBeer = false;
        private bool hasTable = false;
        private bool isActive;
        private bool beerIsFinished;
        private const int TimeToEnter = 1;
        private const int TimeToWalkToTable = 4;
        private const int MaxDrinkTime = 20;
        private const int MinDrinkTime = 10;
        private const int DefaultCheckTime = 1;
        public Patron(string name, ConcurrentQueue<Patron> beerQueue, ConcurrentQueue<Patron> tableQueue)
        {
            SimulationSpeed = 1f;
            beerIsFinished = false;
            actionQueue = new Queue<Action>();
            this.name = name;
            this.tableQueue = tableQueue;
            this.beerQueue = beerQueue;
            Initialize();
        }

        public override void Initialize()
        {
            actionQueue.Enqueue(EnterBar);
            actionQueue.Enqueue(JoinBeerQueue);
            actionQueue.Enqueue(WaitForBeer);
            actionQueue.Enqueue(JoinTableQueue);
            actionQueue.Enqueue(WaitForTable);
            actionQueue.Enqueue(DrinkBeer);
            actionQueue.Enqueue(LeaveBar);
            actionQueue.Enqueue(End);
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
            });
        }

        public override void Pause()
        {
            isActive = false;
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"{Name} took a break"));
        }

        public override void End()
        {
            if (beerIsFinished)
            {
                isActive = false;
                cancellationTokenSource.Cancel();
                LogThis(this, new EventMessage($"{Name} left the bar"));
            }
            else
            {
                actionQueue.Enqueue(End);
            }
            
        }        

        public void EnterBar()
        {
            Bar.PatronsInBar += 1;
            Thread.Sleep((int)(TimeToEnter * PatronSpeed * SimulationSpeed * 1000));
        }

        public void WaitForBeer()
        {
            while (!token.IsCancellationRequested)
            {
                if (hasBeer)
                {
                    LogThis(this, new EventMessage($"{Name} got a beer"));
                    break;
                }
                Thread.Sleep((int)(DefaultCheckTime * PatronSpeed * SimulationSpeed * 1000));
            }
        }

        private void JoinBeerQueue()
        {
            beerQueue.Enqueue(this);
        }

        public void WaitForTable()
        {
            while (!token.IsCancellationRequested)
            {
                if (hasTable)
                {
                    Thread.Sleep((int)(TimeToWalkToTable * PatronSpeed * SimulationSpeed * 1000));
                    LogThis(this, new EventMessage($"{Name} got a table"));
                    break;
                }
                Thread.Sleep((int)(DefaultCheckTime * PatronSpeed * SimulationSpeed * 1000));
            }
        }

        private void JoinTableQueue()
        {
            tableQueue.Enqueue(this);
        }

        public void DrinkBeer()
        {
            var beerDrinkTime = (int)((random.Next(MaxDrinkTime - MinDrinkTime) + MinDrinkTime) * PatronSpeed * SimulationSpeed * 1000);
            Thread.Sleep(beerDrinkTime);
            LogThis(this, new EventMessage($"{Name} downed a beer ({beerDrinkTime/1000}s)"));
            beerIsFinished = true;
        }

        private void LeaveBar()
        {
            Bar.DirtyGlasses++;
            Bar.PatronsInBar--;
            Bar.AvailableTables++;
        }

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