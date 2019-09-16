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
using System.Windows.Threading;

namespace Lab6_Pub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int MAX_OPENTIME = 100;
        public const int MAX_GLASSES = 8;
        public const int SPEED_INCREASE = 1;


        public const int MAX_TABLES = 9;
        public static int actualGlasses = 0;
        public static int actualTables = 0;
        public static int openTime;

        internal int MAX_ENTRYTIME = 15;
        internal int MIN_ENTRYTIME = 10;
        internal int BartenderPourBeerTime = 3; 

        System.Threading.Timer TheTimer = null;

        internal Random random = new Random();
        internal int timeToEntry = 0;
        public static bool open = false;
        internal static Bouncer bouncer = new Bouncer();
        internal static Bartender bartender = new Bartender();
        public static Waitress waitress = new Waitress();
        public BlockingCollection<Patron> patrons = new BlockingCollection<Patron>();
        public BlockingCollection<Patron> wantsBeer = new BlockingCollection<Patron>();


        public CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationTokenSource bouncerCTS = new CancellationTokenSource();
        private CancellationTokenSource bartenderCTS = new CancellationTokenSource();


        public DispatcherTimer timer = new DispatcherTimer();



        public MainWindow()
        {
            InitializeComponent();
            actualGlasses = MAX_GLASSES;
            actualTables = MAX_TABLES;
            int chairs = MAX_TABLES;

            //TODO - Increase speed Button 

            lblGlasses.Content = $"There are {actualGlasses} free Glasses ({MAX_GLASSES} total)";
            lblPatrons.Content = $"There are 0 Patrons in the bar";
            lblTables.Content = $"There are {actualTables} free Tables ({MAX_TABLES} total)";
            btnPauseBartender.Click += BtnPauseBartender_Click;
            btnPauseWaitress.Click += BtnPauseWaitress_Click;
            btnPausePatrons.Click += BtnPausePatrons_Click;
            btnOpenClose.Click += BtnOpenClose_Click;
            btnStopAll.Click += BtnStopAll_Click;
            timer.Tick += timer_Tick;
            btnIncreaseSpeed.Click += IncreaseSpeed_Click; 


            //lbPatrons.ItemsSource = patrons;
            //lbPatrons.DisplayMemberPath = "PatronName";

            // varje gäst ska ha en task - tommy
            //Bartenden ska taa emot request från patron - Tommy
        }

        private void IncreaseSpeed_Click(object sender, RoutedEventArgs e)
        {

            MAX_ENTRYTIME = MAX_ENTRYTIME - (SPEED_INCREASE*3);
            MIN_ENTRYTIME = MIN_ENTRYTIME - (SPEED_INCREASE * 3); 
            Waitress.glassDelay = Waitress.glassDelay - (SPEED_INCREASE);
            Waitress.washDelay = Waitress.washDelay - (SPEED_INCREASE);
            BartenderPourBeerTime = BartenderPourBeerTime - (SPEED_INCREASE);

            if (MAX_ENTRYTIME == 1 || Waitress.glassDelay == 1 || Waitress.washDelay == 1 || BartenderPourBeerTime == 1)
            {
                btnIncreaseSpeed.IsEnabled = false; 
            }
        }

        private void BtnStopAll_Click(object sender, RoutedEventArgs e)
        {
            CloseBar(); 
        }

        private void BtnPausePatrons_Click(object sender, RoutedEventArgs e)
        {
            if (!bouncerCTS.IsCancellationRequested)
            {
                bouncerCTS = new CancellationTokenSource();
                StartBouncer(bouncerCTS.Token);
            } else
            {
                bouncerCTS.Cancel();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            lblOpen.Content = string.Format("00:0{0}:{1}", MAX_OPENTIME / 60, MAX_OPENTIME % 60);
            MAX_OPENTIME--;
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
            open = false;
            timer.Stop();
            StopBouncer();
            StopBartender();
            StopWaitress();
        }


        private void OpenBar()
        {
            open = true;
            timer.Start();
            Task.Run(() =>
            {

                StartWaitress(tokenSource.Token); 
                StartBouncer(bouncerCTS.Token);
                StartBartender(bartenderCTS.Token);

                Thread.Sleep(MAX_OPENTIME * 1000);
                open = false;
            });
        }

        private void StartBartender(CancellationToken token)
        {
            Task.Run(() =>
            {
                while ((open || patrons.Count > 0) && !token.IsCancellationRequested) // Ska vara öppet eller gäster kvar.
                {
                    if (actualGlasses > 0)
                    {
                        bartender.PickUpGlass();
                        actualGlasses -= 1;
                        Dispatcher.Invoke(() => lblGlasses.Content = $"There are {actualGlasses} free Glasses ({MAX_GLASSES} total)");
                        bartender.PourBeer();
                        Thread.Sleep(BartenderPourBeerTime *1000);
                        Dispatcher.Invoke(() => lbBartender.Items.Insert(0, "Poured a Beer"));
                    }
                }
                lbBartender.Items.Insert(0, "Bartendern gick hem");
            });
        }

        public void StartWaitress(CancellationToken token)
        {

            var task = Task.Run(() =>
                {
                    while ((open || patrons.Count > 0) && !token.IsCancellationRequested)
                    {
                       
                        Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress is picking glasses"));
                        waitress.PickUpglasses();
                        Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress is washing glasses"));
                        waitress.WashGlases();
                        Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress put the glasses on the shelf"));
                        waitress.PutOnShelf();
                        Dispatcher.Invoke(() => lblGlasses.Content = $"There are {actualGlasses} free Glasses ({MAX_GLASSES} total)");
                            if (actualGlasses == MAX_GLASSES)
                            {
                                Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress found 0 glasses"));
                            }
                        Thread.Sleep(1000);
                        Dispatcher.Invoke(() => lbWaitress.Items.Clear());
                    }
                    Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress tog en paus"));
                });
        }


        private void StopWaitress()
        {
            tokenSource.Cancel();
        }

        private void StopBartender()
        {
            bartenderCTS.Cancel();
        }
        private void StartBouncer(CancellationToken token)
        {
            Task.Run(() =>
            {
                while (open && !token.IsCancellationRequested)
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
            Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, $"Bouncern släppte in {patron.PatronName}"));
            Dispatcher.Invoke(() => lblPatrons.Content = $"There are {patrons.Count} Patrons in the bar");
            //Dispatcher.Invoke(() => { lbPatrons.Items.Refresh(); } );
            
        }

        private void StopBouncer()
        {
            bouncerCTS.Cancel();
        }

        private void BtnPauseBartender_Click(object sender, RoutedEventArgs e)
        {
            if (bartenderCTS.IsCancellationRequested)
            {
                bartenderCTS = new CancellationTokenSource();
                StartBartender(bartenderCTS.Token);
            } else
            {
                StopBartender();
            }
        }

        private void BtnPauseWaitress_Click(object sender, RoutedEventArgs e)
        {

            if (tokenSource.IsCancellationRequested)
            {
                tokenSource = new CancellationTokenSource();
                StartWaitress(tokenSource.Token);
            }
            else
            {
                tokenSource.Cancel();
                StopWaitress();
            }
        }
    }
}
