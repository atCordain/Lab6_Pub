using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    class Bar
    {
        private const int totalGlassesInBar = 8;
        private const int totalTablesInBar = 9;
        public int TotalGlassesInBar => totalGlassesInBar;
        public int TotalTablesInBar => totalTablesInBar;
        public const int DefaultWaitTime = 1;
        private static int patronsInBar;
        private static bool isBarOpen;
        public static int AvailableGlasses { get; set; }
        public static int AvailableTables { get; set; }
        public static int PatronsInBar => patronsInBar;
        public static bool IsBarOpen => isBarOpen;

        public static int DirtyGlasses { get;  set; }

        // Agents
        private static BlockingCollection<Agent> agents;
        private Bartender bartender;
        private Bouncer bouncer;
        private Waitress waitress;

        // Queues
        public static ConcurrentQueue<Patron> beerQueue = new ConcurrentQueue<Patron>();
        public static ConcurrentQueue<Patron> tableQueue = new ConcurrentQueue<Patron>();

        public Bar()
        {
            agents = new BlockingCollection<Agent>();
            bartender = new Bartender();
            bouncer = new Bouncer(RunPatron);
            waitress = new Waitress();
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
            Run();
        }

        public void CloseBar()
        {
            isBarOpen = false;
            foreach (var agent in agents)
            {
                agent.End();
            }
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
                agent.Cancel();
            }
        }

        public void CancelBartender()
        {
            bartender.Cancel();
        }
        public void CancelWaitress()
        {
            waitress.Cancel();
        }

        public void CancelBouncerAndPatrons()
        {
            bouncer.Cancel();
            foreach (var agent in agents)
            {
                if (agent is Patron patron)
                {
                    patron.Cancel();
                }
            }
        }

        public void RunPatron(Patron patron)
        {
            agents.Add(patron);
            patron.Run();
        }

        internal static void JoinBeerQueue(Patron patron)
        {
            beerQueue.Enqueue(patron);
        }

        internal static void JoinTableQueue(Patron patron)
        {
            tableQueue.Enqueue(patron);
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