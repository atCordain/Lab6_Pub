using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Lab6_Pub
{
    class Bartender
    {
        private const int POUR_BEER_TIME = 3;
        private const int REST_TIME = 1;
        private int currentGlasses;
        public Bartender(int maxGlasses)
        {
            currentGlasses = maxGlasses;
        }

        public int PourBeerTime => POUR_BEER_TIME;

        public int RestTime => REST_TIME;

        public int CurrentGlasses { get => currentGlasses; set => currentGlasses = value; }

        public string PourBeer(Patron patron)
        {
            return $"Poured a beer for {patron.Name}";
        }
        public bool TakeGlass()
        {
            if (currentGlasses > 0)
            {
                currentGlasses -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
