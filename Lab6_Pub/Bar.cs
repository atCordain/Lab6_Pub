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
        private const int totalGlassesInBar = 8;
        private const int totalTablesInBar = 9;
        public static int TotalGlassesInBar => totalGlassesInBar;
        public static int TotalTablesInBar => totalTablesInBar;
        public const int DefaultWaitTime = 1;
        private static int patronsInBar;
        private static bool isBarOpen;
        public static int AvailableGlasses { get; set; }
        public static int AvailableTables { get; set; }
        public static int PatronsInBar { get; set; }
        public static bool IsBarOpen => isBarOpen;

        private const int MaxOpenTime = 120;
        public static int OpenTimeLeft;

        public static int DirtyGlasses { get;  set; }

        // Agents
        private static List<Agent> agents;
        private Bartender bartender;
        private Bouncer bouncer;
        private Waitress waitress;

        // Queues
        public static ConcurrentQueue<Patron> beerQueue = new ConcurrentQueue<Patron>();
        public static ConcurrentQueue<Patron> tableQueue = new ConcurrentQueue<Patron>();

        public System.Timers.Timer timer;


        // TODO: Speed increase, Use cases ()
        public Bar()
        {
        
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
            isBarOpen = true;
            OpenTimeLeft = MaxOpenTime;
            StartOpenTimer();
            Run();
        }

        private void StartOpenTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += OpenTimer_Elapsed;
            timer.Start();
        }
        private void OpenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OpenTimeLeft--;
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

            foreach (var agent in agents)
            {
                if (agent is Patron patron)
                {
                    if (patron.IsActive) patron.Pause();
                    else patron.Run();
                }
            }
        }

        internal static bool IsBarEmpty()
        {
            foreach (var agent in agents)
            {
                if (agent is Patron)
                {
                    return false;
                }
            }
            return true;
        }
    }
}