using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Bouncer : Agent
    {
        public static event EventHandler LogThis;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;

        private int MAX_ENTRYTIME = 15;
        private int MIN_ENTRYTIME = 10;
        private int SleepTime = 5000;

        private Random random = new Random();
        private Patron patron;
        private double speed = 1;

        private Action<Patron> addPatronToAgents;

        public Bouncer(Action<Patron> addPatronToAgents)
        {
            this.addPatronToAgents = addPatronToAgents;
        }

        public Bouncer() { }

        private string CheckID()
        {
            string[] names = { "Johan", "Tommy", "Petter", "Calle", "Kolle", "Per", "Nisse", "Frippe", "Machmud", "Jonna", "Sara" };
            return names[random.Next(names.Length)];
        }

        public List<Patron> CreatePatron( int numberPatrons)
        {
            List<Patron> patrons = new List<Patron>(); 

            for (int i = 0; i < numberPatrons; i++)
            {
                patrons.Add(new Patron(CheckID()));
            }
            Thread.Sleep(GetSleepTime());
            return patrons;
        }

        public void CancelPatrons()
        {
            cancellationTokenSource.Cancel();
        }

        public void SetSpeed(double speed)
        {
            this.speed = speed;
        }

        public int GetSleepTime() => (int)(random.Next(MAX_ENTRYTIME - MIN_ENTRYTIME) + MIN_ENTRYTIME * speed * 1000);

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;
            Task.Run(() => 
            {
                while (!token.IsCancellationRequested && Bar.IsBarOpen)
                {
                    LetPatronIn();
                    Thread.Sleep(SleepTime);
                }
            }, token);
        }

        private void LetPatronIn()
        {
            var patron = new Patron(GetPatronName());
            addPatronToAgents(patron);            ;
            LogThis(this, new EventMessage($"Bouncer let {patron.Name} into the bar"));
        }
        private string GetPatronName()
        {
            string[] names = { "Johan", "Tommy", "Petter", "Calle", "Kolle", "Per", "Nisse", "Frippe", "Machmud", "Jonna", "Sara" };
            return names[random.Next(names.Length)];
        }

        public override void Cancel()
        {
            cancellationTokenSource.Cancel();
            LogThis(this, new EventMessage($"Bouncer went home (Cancel)"));
        }

        public override void End()
        {
            LogThis(this, new EventMessage($"Bouncer went home (End)"));
        }
    }
}