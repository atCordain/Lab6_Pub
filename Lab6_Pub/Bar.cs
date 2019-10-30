using System;
using System.Collections.Concurrent;
using System.Timers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Bar
    {
        //Undvik statiska fält och properties som pesten, dom skadar skalningsbarheten av ett projekt på sikt. 
        // FIXAT! ^^
        public int TotalGlassesInBar { get => totalGlassesInBar; set => totalGlassesInBar = value; }
        public int TotalTablesInBar { get => totalTablesInBar; set => totalTablesInBar = value; }
        public int AvailableGlasses { get; set; }
        public int DirtyGlasses { get;  set; }
        public int AvailableTables { get; set; }
        public int PatronsInBar { get; set; }
        public bool IsBarOpen => isBarOpen;
        public int MaxOpenTime { get => maxOpenTime; set => maxOpenTime = value; }
        public int OpenTimeLeft { get => openTimeLeft; set => openTimeLeft = value; }

        public enum StartCondition { Standard, TwentyGlassThreeChairs, TwentyChairsFiveGlass, GuestStayDoubled, WaitresWorksDoubleSpeed, BarOpenFiveMinuites, CouplesNight, PatronBusLoad }
        public StartCondition startCondition;
        public System.Timers.Timer timer;
        public const int DefaultWaitTime = 1;

        private int maxOpenTime = 120;
        private int openTimeLeft;
        private float simulationSpeed;

        private int totalGlassesInBar = 8;
        private int totalTablesInBar = 9;
        private  int patronsInBar;

        private bool isBarOpen;

        private List<Agent> agents;
        private Bartender bartender;
        private Bouncer bouncer;
        private Waitress waitress;

        private ConcurrentQueue<Patron> beerQueue = new ConcurrentQueue<Patron>();
        private ConcurrentQueue<Patron> tableQueue = new ConcurrentQueue<Patron>();

        internal int GetNumberOfPatronsInTableQueue()
        {
            return tableQueue.Count;
        }

        private bool updateTime;

        public Bar()
        {
            startCondition = StartCondition.Standard;
            simulationSpeed = 1f;
            timer = new System.Timers.Timer();
            
            agents = new List<Agent>();
            bartender = new Bartender(this);
            bouncer = new Bouncer(this);
            waitress = new Waitress(this);

            agents.Add(bartender);
            agents.Add(bouncer);
            agents.Add(waitress);
            
            AvailableGlasses = totalGlassesInBar;
            AvailableTables = totalTablesInBar;

            isBarOpen = false;
        }

        internal void AddAgentToAgentList(Agent agent)
        {
            agents.Add(agent);
        }

        internal int GetNumberOfPatronsInBeerQueue()
        {
            return beerQueue.Count;
        }

        internal Patron GetPatronToServeTable()
        {
            Patron patron;
            tableQueue.TryDequeue(out patron);
            return patron;
        }

        public void OpenBar()
        {
            switch (startCondition)
            {
                case StartCondition.Standard:
                    break;
                case StartCondition.TwentyGlassThreeChairs:
                    TotalGlassesInBar = 20;
                    AvailableGlasses = 20;
                    TotalTablesInBar = 3;
                    AvailableTables = 3;
                    break;
                case StartCondition.TwentyChairsFiveGlass:
                    TotalGlassesInBar = 5;
                    AvailableGlasses = 5;
                    TotalTablesInBar = 20;
                    AvailableTables = 20;
                    break;
                case StartCondition.GuestStayDoubled:
                    Patron.PatronSpeed = 2;
                    break;
                case StartCondition.WaitresWorksDoubleSpeed:
                    waitress.SimulationSpeed = waitress.SimulationSpeed / 2;
                    break;
                case StartCondition.BarOpenFiveMinuites:
                    MaxOpenTime = 300;
                    break;
                case StartCondition.CouplesNight:
                    bouncer.couplesNight = true;
                    break;
                case StartCondition.PatronBusLoad:
                    bouncer.SimulationSpeed = bouncer.SimulationSpeed * 2;
                    bouncer.busLoad = true;
                    break;
            }
            isBarOpen = true;
            OpenTimeLeft = MaxOpenTime;
            StartOpenTimer();
            Run();
        }

        internal void JoinBeerQueue(Patron patron)
        {
            beerQueue.Enqueue(patron);
        }
        
        public void JoinTableQueue(Patron patron)
        {
            tableQueue.Enqueue(patron);
        }

        public Patron GetPatronToServeBeer()
        {
            Patron patron;
            beerQueue.TryDequeue(out patron);
            return patron;
        }

        private void StartOpenTimer()
        {
            // Interval is 500 and time update is every other timer.Elapsed because, 
            // the UI also subscribe to timer.Elapsed and the UI update needs to be more often than 1 per second.
            timer.Interval = 500;
            timer.Elapsed += OpenTimer_Elapsed;
            updateTime = false;
            timer.Start();
        }
        private void OpenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (updateTime)
            {
                OpenTimeLeft--;
                updateTime = false;
            }
            else updateTime = true;
            
            if (OpenTimeLeft <= 0)
            {
                timer.Stop();
                CloseBar();
            }
        }

        public void CloseBar()
        {
            isBarOpen = false;
            bouncer.End();
        }

        public void Run()
        {
            foreach (var agent in agents)
            {
                agent.Run();
            }
        }

        public void Cancel()
        {
            foreach (var agent in agents)
            {
                if (agent.IsActive)
                {
                    agent.Pause(); 
                }
            }
        }

        public void PauseResumeBartender()
        {
            if (bartender.IsActive) bartender.Pause();
            else bartender.Run();
        }
        public void PauseResumeWaitress()
        {
            if (waitress.IsActive) waitress.Pause();
            else waitress.Run();
        }

        public void PauseResumeBouncerPatrons()
        {
            if (bouncer.IsActive) bouncer.Pause();
            else bouncer.Run();

            // ToArray() för att undvika System.InvalidOperationException: 'Collection was modified; enumeration operation may not execute.'
            foreach (var agent in agents.ToArray())
            {
                if (agent is Patron patron)
                {
                    if (patron.IsActive) patron.Pause();
                    else patron.Run();
                }
            }
        }

        public void IncreaseSimulationSpeed()
        {
            simulationSpeed = simulationSpeed / 2;
            timer.Interval = 1000 * simulationSpeed;
            foreach (var agent in agents)
            {
                agent.SimulationSpeed = simulationSpeed;
            }
        }
    }
}