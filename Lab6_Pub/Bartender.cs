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
        private double speed = 1;

        public Bartender()
        {

        }

        public string PourBeer(Patron patron)
        {
            Thread.Sleep((int)(POUR_BEER_TIME * speed * 1000));
            patron.BeerDelivery();
            return $"Poured a beer for {patron.Name}";
        }

        public void TakeGlass()
        {
            MainWindow.availableGlasses -= 1;
        }

        public void SetSpeed(double speed)
        {
            this.speed = speed;
        }
    }
}
