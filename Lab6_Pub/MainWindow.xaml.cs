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
        public const int MAX_CHAIRS = 9;
        public static int actualGlasses = 0; 
        public static int openTime;
        internal const int MAX_ENTRYTIME = 10;
        internal const int MIN_ENTRYTIME = 3;
        internal Random random = new Random();
        internal int timeToEntry = 0;
        public int openTime;
        public static bool open = false;
        public static Bouncer bouncer = new Bouncer();
        public BlockingCollection<Patron> patrons = new BlockingCollection<Patron>();
        public BlockingCollection<Patron> wantsBeer = new BlockingCollection<Patron>();
        internal CancellationToken bouncerCancellation = new CancellationTokenSource().Token;

        public MainWindow()
        {
            InitializeComponent();
            int glasses = MAX_GLASSES;
            int chairs = MAX_CHAIRS;
            openTime = MAX_OPENTIME;

            Waitress waitress1 = new Waitress();

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
        }

        private void OpenBar()
        {
            // STARTA THREADS 
            open = true;
            Task.Run(() =>
            {
                StartBouncer();
                Thread.Sleep(MAX_OPENTIME * 1000);
                open = false;
            });
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

                Thread pickUpGlasses = new Thread(Waitress.PickUpglasses);
                Thread washGlasses = new Thread(Waitress.WashGlases);
                Thread putOnShelf = new Thread(Waitress.PutOnShelf);

                pickUpGlasses.Start();
                washGlasses.Start();
                putOnShelf.Start();
            }
        }
    }
}
