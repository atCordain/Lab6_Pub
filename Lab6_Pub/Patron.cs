using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6_Pub
{
    public class Patron
    {
        private string name;
        private Random random = new Random();
        private bool hasBeer = false;
        private bool hasTable = false;
        private const int BEER_CHECK_TIME = 1;
        private const int ENTER_WAIT_TIME = 1;
        private const int WALK_TO_TABLE_TIME = 4;
        private const int MAX_DRINKTIME = 20;
        private const int MIN_DRINKTIME = 10;
        private const int DEFAULT_CHECK_TIME = 1;
        private List<Patron> beerQueue;
        private CancellationToken cancellationToken;
        private double speed = 1;

        public Patron(string name)
        {
            this.name = name;
        }

        public void EnterBar()
        {
            Thread.Sleep((int)(ENTER_WAIT_TIME * speed * 1000));
        }

        public void WaitForBeer()
        {
            MainWindow.JoinBeerQueue(this);
            while (!hasBeer)
            {
                Thread.Sleep((int)(BEER_CHECK_TIME * speed * 1000));
            }
        }

        public void LookForTable()
        {
            while (!hasTable)
            {
                if (MainWindow.availableTables > 0) 
                {
                    hasTable = true;
                    MainWindow.availableTables -= 1;
                }
                Thread.Sleep((int)(DEFAULT_CHECK_TIME * speed * 1000));
            }
            Thread.Sleep((int)(WALK_TO_TABLE_TIME * speed *  1000));
        }

        public string Sit()
        {
            return $"{Name} sat at a table";
        }

        public void DrinkBeer()
        {
            // slumpa 10-20 sec.
            Thread.Sleep((int)((random.Next(MAX_DRINKTIME - MIN_DRINKTIME) + MIN_DRINKTIME) * speed * 1000));
        }
        public string Leave()
        {
            MainWindow.dirtyGlasses += 1;
            MainWindow.availableTables += 1;
            return $"{Name} left the bar.";

           

        }
        public void SetSpeed(double speed)
        {
            this.speed = speed;
        }

        public string Name { get => name; set => name = value; }
        public List<Patron> BeerQueue { get => beerQueue; set => beerQueue = value; }
        public CancellationToken CancellationToken { get => cancellationToken; set => cancellationToken = value; }

        public void BeerDelivery()
        {
            hasBeer = true;
        }
    }
}