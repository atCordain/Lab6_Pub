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
        public override bool IsActive { get => isActive; set => isActive = value; }
        public override float SimulationSpeed { get; set; }
        
        public bool couplesNight = false;
        public bool busLoad = false;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;

        private bool isActive;
        private int maxEntryTime = 15;
        private int minEntryTime = 10;

        private Random random = new Random();
        private List<Agent> agents;
        private ConcurrentQueue<Patron> beerQueue;
        private ConcurrentQueue<Patron> tableQueue;

        public Bouncer(List<Agent> agents, ConcurrentQueue<Patron> beerQueue, ConcurrentQueue<Patron> tableQueue)
        {
            SimulationSpeed = 1;
            this.agents = agents;
            this.beerQueue = beerQueue;
            this.tableQueue = tableQueue;
        }

        public override void Initialize()
        {
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
                    Thread.Sleep((int)(random.Next(maxEntryTime - minEntryTime) + minEntryTime * SimulationSpeed * 1000));
                }
                if (isActive) End();
            }, token);
        }

        private void LetPatronIn()
        {
            if (busLoad && (Bar.maxOpenTime - Bar.openTimeLeft) > 20)
            {
                for (int i = 0; i < 15; i++) 
                {
                    CreateAndRunPatron();
                } 
                busLoad = false;
            }
            else CreateAndRunPatron();
            if (couplesNight) CreateAndRunPatron();
        }

        private void CreateAndRunPatron()
        {
            var patron = new Patron(GetPatronName(), beerQueue, tableQueue);
            agents.Add(patron);
            patron.SimulationSpeed = SimulationSpeed;
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
            LogThis(this, new EventMessage($"Bouncer went home"));
            isActive = false;
        }
    }
}