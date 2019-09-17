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
    public class Bartender
    {
        private const int POUR_BEER_TIME = 3;
        private const int REST_TIME = 1;

        public Bartender()
        {

        }

        public string PourBeer(Patron patron)
        {
            Thread.Sleep(POUR_BEER_TIME * 1000);
            return $"Poured a beer for {patron.Name}";
        }

        public bool TakeGlass()
        {
            if (MainWindow.availableGlasses > 0)
            {
                MainWindow.availableGlasses -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
