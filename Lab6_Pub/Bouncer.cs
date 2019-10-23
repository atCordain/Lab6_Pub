using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Bouncer : Agent
    {
        public static event EventHandler LogThis;
        private bool isActive;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;

        private int MaxEntryTime = 15;
        private int MinEntryTime = 10;

        private Random random = new Random();
        private List<Agent> agents;
        private ConcurrentQueue<Patron> beerQueue;
        private ConcurrentQueue<Patron> tableQueue;

        public override bool IsActive { get => isActive; set => isActive = value; }

        public Bouncer(List<Agent> agents, ConcurrentQueue<Patron> beerQueue, ConcurrentQueue<Patron> tableQueue)
        {
            this.agents = agents;
            this.beerQueue = beerQueue;
            this.tableQueue = tableQueue;
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            isActive = true;
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;
            Task.Run(() => 
            {
                while (!token.IsCancellationRequested && Bar.IsBarOpen)
                {
                    LetPatronIn();
                    Thread.Sleep(random.Next(MaxEntryTime - MinEntryTime) + MinEntryTime * 1000);
                }
                if (isActive) End();
            }, token);
        }

        private void LetPatronIn()
        {
            var patron = new Patron(GetPatronName(), beerQueue, tableQueue);
            agents.Add(patron);
            patron.Run();
            LogThis(this, new EventMessage($"{patron.Name} entered the bar"));
        }
        private string GetPatronName()
        {
            string[] names = { "Johan", "Tommy", "Petter", "Calle", "Kolle", "Per", "Nisse", "Frippe", "Machmud", "Jonna", "Sara" };
            return names[random.Next(names.Length)];
        }

        public override void Pause()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"Bouncer took a break"));
            isActive = false;
        }

        public override void End()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"Bouncer went home (End)"));
            isActive = false;
        }
    }
}