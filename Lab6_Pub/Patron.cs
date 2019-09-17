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
        private const int BEER_CHECK_TIME = 1;
        private const int ENTER_WAITTIME = 1;
        private const int WALK_TO_TABLETIME = 4;
        private const int MAX_DRINKTIME = 20;
        private const int MIN_DRINKTIME = 10;
        private BlockingCollection<Patron> beerQueue;
        private CancellationToken cancellationToken;
        
        public Patron(string name)
        {
            this.name = name;
        }

        private void EnterBar()
        {
            //PatronSays($"{name} har gått in på baren"));
            //Thread.Sleep(ENTER_WAITTIME * 1000);
        }

        public void GetBeer()
        {
            BeerQueue.Add(this);
            while (!hasBeer)
            {
                Thread.Sleep(BEER_CHECK_TIME * 1000);
            }
        }

        public void LookForTable()
        {
            Thread.Sleep(WALK_TO_TABLETIME * 1000);
        }

        public void Sit()
        {

        }



        public void DrinkBeer()
        {
            // slumpa 10-20 sec.
            Thread.Sleep((random.Next(MAX_DRINKTIME + MIN_DRINKTIME) - MIN_DRINKTIME) * 1000);
        }
        public void Leave()
        {

        }

        public string Name { get => name; set => name = value; }
        public BlockingCollection<Patron> BeerQueue { get => beerQueue; set => beerQueue = value; }
        public CancellationToken CancellationToken { get => cancellationToken; set => cancellationToken = value; }

        public int EnterWaitTime => ENTER_WAITTIME;

        public void BeerDelivery()
        {
            hasBeer = true;
        }
    }
}