using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Concurrent;
using System.Threading;

namespace Lab6_Pub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int TIMESCALE = 1;
        public const int MAX_OPENTIME = 10;
        public const int MAX_GLASSES = 8;
        public const int MAX_TABLES = 9;
        public static int actualGlasses = 0;
        public static int actualTables = 0;
        public static int openTime;
        internal const int MAX_ENTRYTIME = 10;
        internal const int MIN_ENTRYTIME = 3;
        internal Random random = new Random();
        internal int timeToEntry = 0;
        public static bool open = false;
        internal static Bouncer bouncer = new Bouncer();
        internal static Bartender bartender = new Bartender();
        public BlockingCollection<Patron> patrons = new BlockingCollection<Patron>();
        public BlockingCollection<Patron> wantsBeer = new BlockingCollection<Patron>();

        internal CancellationToken bouncerCancellation = new CancellationTokenSource().Token;

        public MainWindow()
        {
            InitializeComponent();
            actualGlasses = MAX_GLASSES;
            actualTables = MAX_TABLES;
            int chairs = MAX_TABLES;


            Waitress waitress1 = new Waitress();

            lblGlasses.Content = $"There are {actualGlasses} free Glasses ({MAX_GLASSES} total)";
            lblPatrons.Content = $"There are 0 Patrons in the bar";
            lblTables.Content = $"There are {actualTables} free Tables ({MAX_TABLES} total)"; 
            btnPauseBartender.Click += BtnPauseBartender_Click;
            btnPauseWaitress.Click += BtnPauseWaitress_Click;
            btnPausePatrons.Click += BtnPausePatrons_Click;
            btnOpenClose.Click += BtnOpenClose_Click;
            //lbPatrons.ItemsSource = patrons;
            //lbPatrons.DisplayMemberPath = "PatronName";


        }


        private void BtnPausePatrons_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnPauseWaitress_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnOpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (open)
            {
                CloseBar();
            }
            else
            {
                OpenBar();
            }
             
        }

        private void CloseBar()
        {
            // AVVAKTA THREADS
            open = false;
            StopBouncer();
            StopBartender();
        }

        private void OpenBar()
        {
            // STARTA THREADS 
            open = true;
            Task.Run(() =>
            {
                StartBouncer();
                StartBartender();
                Thread.Sleep(MAX_OPENTIME * 1000);
                open = false;
            });
        }

        private void StartBartender()
        {

            Task.Run(() =>
            {
                while (open) // Ska vara öppet eller gäster kvar.
                {
                    if (actualGlasses > 0)
                    {
                        bartender.PickUpGlass();
                        actualGlasses -= 1;
                        Dispatcher.Invoke(() => lblGlasses.Content = $"There are {actualGlasses} free Glasses ({MAX_GLASSES} total)");
                        bartender.PourBeer();
                        Thread.Sleep(3000);
                        Dispatcher.Invoke(() => lbBartender.Items.Insert(0, "Poured a Beer"));
                    }
                }
            });
        }

        private void StopBartender()
        {
            throw new NotImplementedException();
        }
        private void StartBouncer()
        {
            Task.Run(() =>
            {
                while (open)
                {
                    timeToEntry = random.Next(MAX_ENTRYTIME - MIN_ENTRYTIME) + MIN_ENTRYTIME *1000;
                    StartPatron(bouncer.CreatePatron());                   
                    Thread.Sleep(timeToEntry);
                    
                }
                Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, "Bouncer gick hem"));
            });
        }

        private void StartPatron(Patron patron)
        {
            patrons.Add(patron);
            Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, patron.PatronName));
            Dispatcher.Invoke(() => lblPatrons.Content = $"There are {patrons.Count} Patrons in the bar");
            //Dispatcher.Invoke(() => { lbPatrons.Items.Refresh(); } );
            
        }

        private void StopBouncer()
        {
            //bouncerCancellation.Cancel();
        }

        private void BtnPauseBartender_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnPauseWaitress_Click_1(object sender, RoutedEventArgs e)
        {
            if (open)
            {
                Waitress.StopWaitress(); 
            }
            else
            {
                Waitress.StartWaitress();

                //Thread pickUpGlasses = new Thread(Waitress.PickUpglasses);
                //Thread washGlasses = new Thread(Waitress.WashGlases);
                //Thread putOnShelf = new Thread(Waitress.PutOnShelf);

                //pickUpGlasses.Start();
                //washGlasses.Start();
                //putOnShelf.Start();
            }
        }
    }
}
