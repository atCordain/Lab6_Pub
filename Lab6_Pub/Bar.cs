using System;
using System.Collections.Concurrent;
using System.Timers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    class Bar
    {
        public static int TotalGlassesInBar { get => totalGlassesInBar; set => totalGlassesInBar = value; }
        public static int TotalTablesInBar { get => totalTablesInBar; set => totalTablesInBar = value; }
        public static int AvailableGlasses { get; set; }
        public static int DirtyGlasses { get;  set; }
        public static int AvailableTables { get; set; }
        public static int PatronsInBar { get; set; }
        public static bool IsBarOpen => isBarOpen;
        public enum StartCondition { Standard, TwentyGlassThreeChairs, TwentyChairsFiveGlass, GuestStayDoubled, WaitresWorksDoubleSpeed, BarOpenFiveMinuites, CouplesNight, PatronBusLoad }
        public StartCondition startCondition;
        
        public static int maxOpenTime = 120;
        public static int openTimeLeft;
        public const int DefaultWaitTime = 1;
        private float simulationSpeed;

        private static int totalGlassesInBar = 8;
        private static int totalTablesInBar = 9;
        private static int patronsInBar;

        private static bool isBarOpen;

        private static List<Agent> agents;
        private Bartender bartender;
        private Bouncer bouncer;
        private Waitress waitress;

        public System.Timers.Timer timer;
        public static ConcurrentQueue<Patron> beerQueue = new ConcurrentQueue<Patron>();
        public static ConcurrentQueue<Patron> tableQueue = new ConcurrentQueue<Patron>();
        private bool updateTime;

        public Bar()
        {
            startCondition = StartCondition.Standard;
            simulationSpeed = 1f;
            timer = new System.Timers.Timer();
            
            agents = new List<Agent>();
            bartender = new Bartender(beerQueue);
            bouncer = new Bouncer(agents, beerQueue, tableQueue);
            waitress = new Waitress(tableQueue);
            agents.Add(bartender);
            agents.Add(bouncer);
            agents.Add(waitress);
            
            AvailableGlasses = totalGlassesInBar;
            AvailableTables = totalTablesInBar;

            isBarOpen = false;
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
                    maxOpenTime = 300;
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
            openTimeLeft = maxOpenTime;
            StartOpenTimer();
            Run();
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
                openTimeLeft--;
                updateTime = false;
            }
            else updateTime = true;
            
            if (openTimeLeft <= 0)
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

            foreach (var agent in agents)
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