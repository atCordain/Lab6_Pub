using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Patron : Agent
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;
        
        public static event EventHandler LogThis;

        private Queue<Action> actions;

        private string name;
        private Random random = new Random();
        private bool hasBeer = false;
        private bool hasTable = false;
        private const int TimeToEnter = 1;
        private const int TimeToWalkToTable = 4;
        private const int MaxDrinkTime = 20;
        private const int MinDrinkTime = 10;
        private const int DefaultCheckTime = 1;

        public Patron(string name)
        {
            actions = new Queue<Action>();
            this.name = name;
            Initialize();
        }

        public override void Initialize()
        {
            actions.Enqueue(EnterBar);
            actions.Enqueue(WaitForBeer);
            actions.Enqueue(LookForTable);
            actions.Enqueue(DrinkBeer);
            actions.Enqueue(LeaveBar);
        }

        public override void Run()
        {
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;

            Action action;

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested && actions.Count > 0)
                {
                    action = actions.Dequeue();
                    action();
                }

            }, token);
        }

        public override void Cancel()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"{Name} left the bar (Cancel)"));
        }

        public override void End()
        {
            //throw new NotImplementedException();
        }        

        public void EnterBar()
        {
            Thread.Sleep((int)(TimeToEnter * 1000));
            LogThis(this, new EventMessage($"{Name} waves hello"));
        }

        public void WaitForBeer()
        {
            Bar.JoinBeerQueue(this);
            while (!hasBeer)
            {
                Thread.Sleep((int)(DefaultCheckTime * 1000));
            }
            LogThis(this, new EventMessage($"{Name} got a beer"));
        }

        public void LookForTable()
        {
            Bar.JoinTableQueue(this);
            while (!hasTable)
            {
                Thread.Sleep((int)(DefaultCheckTime * 1000));
            }
            Thread.Sleep((int)(TimeToWalkToTable * 1000));
            LogThis(this, new EventMessage($"{Name} got a table"));
        }


        public void DrinkBeer()
        {
            // slumpa 10-20 sec.
            var beerDrinkTime = (int)((random.Next(MaxDrinkTime - MinDrinkTime) + MinDrinkTime) * 1000);
            Thread.Sleep(beerDrinkTime);
            LogThis(this, new EventMessage($"{Name} downed a beer in {beerDrinkTime} seconds"));

        }
        private void LeaveBar()
        {
            Bar.DirtyGlasses += 1;
            LogThis(this, new EventMessage($"{Name} left the bar"));
        }

        public string Name { get => name; set => name = value; }

        public void GiveBeer()
        {
            hasBeer = true;
        }

        public void GiveTable()
        {
            hasTable = true;
        }
        //public string Leave()
        //{
        //    MainWindow.dirtyGlasses += 1;
        //    MainWindow.availableTables += 1;
        //    return $"{Name} left the bar.";
        //}
        //public string Sit()
        //{
        //    return $"{Name} sat at a table";
        //}
        //public void SetSpeed(double speed)
        //{
        //    this.speed = speed;
        //}
        //public List<Patron> BeerQueue { get => beerQueue; set => beerQueue = value; }
        //public CancellationToken CancellationToken { get => cancellationToken; set => cancellationToken = value; }
    }
}