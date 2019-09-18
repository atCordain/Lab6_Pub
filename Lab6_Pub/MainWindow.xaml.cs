using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace Lab6_Pub
{
    public partial class MainWindow : Window
    {
        public const int MAX_OPENTIME = 100;
        public const int MAX_GLASSES = 8;
        public const int MAX_TABLES = 9; 
        private const int DEFAULT_WAIT_TIME = 1;
        private const int PATRONS_PER_ENTRY = 1; 

        public static int availableGlasses = 0;
        public static int availableTables = 0;
        public static int dirtyGlasses = 0;
        public static int maxOpenTime = 0;
        public static int timeRun = 0; 

        public int openTime;

        private double simulationSpeed = 1;

        public bool bartenderIsWaiting = false;
        public static bool open = false;
        private bool busLoadCase = false;

        private Bouncer bouncer = new Bouncer();
        private Waitress waitress = new Waitress();
        private Bartender bartender = new Bartender();
        
        public List<Patron> patrons = new List<Patron>();
        public static List<Patron> beerQueue = new List<Patron>();

        public CancellationTokenSource waitressCTS = new CancellationTokenSource();
        private CancellationTokenSource bouncerCTS = new CancellationTokenSource();
        private CancellationTokenSource bartenderCTS = new CancellationTokenSource();

        public DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            availableGlasses = MAX_GLASSES;
            availableTables = MAX_TABLES;
            maxOpenTime = MAX_OPENTIME; 

            lblGlasses.Content = $"There are {availableGlasses} free Glasses ({MAX_GLASSES} total)";
            lblPatrons.Content = $"There are 0 Patrons in the bar";
            lblTables.Content = $"There are {availableTables} free Tables ({MAX_TABLES} total)";
          
            btnPauseBartender.Click += BtnPauseBartender_Click;
            btnPauseWaitress.Click += BtnPauseWaitress_Click;
            btnPausePatrons.Click += BtnPausePatrons_Click;
            btnOpenClose.Click += BtnOpenClose_Click;
            btnStopAll.Click += BtnStopAll_Click;
            btnIncreaseSpeed.Click += IncreaseSpeed_Click; 
            timer.Tick += timer_Tick;  
        }

        private void IncreaseSpeed_Click(object sender, RoutedEventArgs e)
        {
            simulationSpeed -= simulationSpeed * 0.5;
            bartender.SetSpeed(simulationSpeed);
            waitress.SetSpeed(simulationSpeed);
            bouncer.SetSpeed(simulationSpeed);
            foreach(var patron in patrons)
            {
                patron.SetSpeed(simulationSpeed);
            }

            if (simulationSpeed < 0.3) btnIncreaseSpeed.IsEnabled = false; 
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
            if (open)
            {
                timeRun += 1; 
                timer.Interval = new TimeSpan(0, 0, 1);
                lblOpen.Content = string.Format("00:0{0}:{1}", maxOpenTime / 60, maxOpenTime % 60);
                maxOpenTime--;

            } else
            {
                timer.Stop();
                lblOpen.Content = string.Format("00:00:00");

            }
            
        }
        
        private void BtnOpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (open) CloseBar();
            else OpenBar();
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
                StartWaitress(waitressCTS.Token); 
                StartBouncer(bouncerCTS.Token);
                StartBartender(bartenderCTS.Token);
                Thread.Sleep(MAX_OPENTIME * 1000);
                open = false;
            });
        }

        private void StartBouncer(CancellationToken token)
        {
            Task.Run(() =>
            {
                while (open && !token.IsCancellationRequested)
                {
                    List<Patron> createPatron; 

                    if (timeRun > 20 && busLoadCase)
                    {
                        createPatron = bouncer.CreatePatron(15);
                        busLoadCase = false; 
                    }
                    else
                    {
                        createPatron = bouncer.CreatePatron(PATRONS_PER_ENTRY);
                    }

                    foreach (var patron in createPatron)
                    {
                        patron.BeerQueue = beerQueue;
                        patron.CancellationToken = bouncerCTS.Token;
                        StartPatron(patron);
                    }                   
                }
                Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, "Bouncer gick hem"));
            });
        }

        private void StartBartender(CancellationToken token)
        {
            var task = Task.Run(() =>
            {
                while ((open || patrons.Count > 0) && !token.IsCancellationRequested)
                {
                    if (availableGlasses > 0 && beerQueue.Count > 0)
                    {
                        bartenderIsWaiting = false;
                        Dispatcher.Invoke(() => lbBartender.Items.Insert(0, "Bartender goes to shelf"));
                        bartender.TakeGlass();
                        Dispatcher.Invoke(() => lblGlasses.Content = $"There are {availableGlasses} free Glasses ({MAX_GLASSES} total)");
                        var patron = beerQueue.ElementAt(0); 
                        bartender.PourBeer(patron);
                        beerQueue.RemoveAt(0); 
                        patron.BeerDelivery();
                        Dispatcher.Invoke(() => lbBartender.Items.Insert(0, $"Poured a beer for {patron.Name}"));
                    }
                    else if (!bartenderIsWaiting)
                    {
                        Dispatcher.Invoke(() => lbBartender.Items.Insert(0, "Waiting in the bar"));
                        bartenderIsWaiting = true;
                    }
                    Thread.Sleep(DEFAULT_WAIT_TIME* 1000);
                }
                Dispatcher.Invoke(() => lbBartender.Items.Insert(0, "Bartendern gick hem"));
            });
        }
        public void StartWaitress(CancellationToken token)
        {
            Task.Run(() =>
            {
                while ((open || patrons.Count > 0 || availableGlasses < MAX_GLASSES) && !token.IsCancellationRequested)
                {
                    if (dirtyGlasses > 0)
                    {
                        Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress is picking glasses"));
                        waitress.PickUpglasses(ref dirtyGlasses);
                        Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress is washing glasses"));
                        waitress.WashGlases();
                        Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, "Waitress put the glasses on the shelf"));
                        availableGlasses += waitress.PutOnShelf();
                        Dispatcher.Invoke(() => lblGlasses.Content = $"There are {availableGlasses} free Glasses ({MAX_GLASSES} total)");
                    }
                    Thread.Sleep(DEFAULT_WAIT_TIME * 1000);
                }
                Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, waitress.Leave())); 
            });
        }

        private void StartPatron(Patron patron)
        {
            Task.Run(() =>
            {
                if(!patron.CancellationToken.IsCancellationRequested)patrons.Add(patron);
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lblPatrons.Content = $"There are {patrons.Count} Patrons in the bar");
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, $"{patron.Name} goes to the bar."));
                if(!patron.CancellationToken.IsCancellationRequested)patron.EnterBar();
                if(!patron.CancellationToken.IsCancellationRequested)patron.WaitForBeer();
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, $"{patron.Name} looks for chair"));
                if(!patron.CancellationToken.IsCancellationRequested)patron.LookForTable();
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, patron.Sit()));
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lblTables.Content = $"There are {availableTables} free Tables ({MAX_TABLES} total)");
                if(!patron.CancellationToken.IsCancellationRequested)patron.DrinkBeer();
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, patron.Leave()));
                if(!patron.CancellationToken.IsCancellationRequested)patrons.Remove(patron);
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lblPatrons.Content = $"There are {patrons.Count} Patrons in the bar");
                if(!patron.CancellationToken.IsCancellationRequested)Dispatcher.Invoke(() => lblTables.Content = $"There are {availableTables} free Tables ({MAX_TABLES} total)");
            });
        }

        private void StopBouncer()
        {
            bouncerCTS.Cancel();
        }

        private void StopWaitress()
        {
            waitressCTS.Cancel();
        }

        private void StopBartender()
        {
            bartenderCTS.Cancel();
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
            if (waitressCTS.IsCancellationRequested)
            {
                waitressCTS = new CancellationTokenSource();
                StartWaitress(waitressCTS.Token);
            }
            else
            {
                waitressCTS.Cancel();
                StopWaitress();
            }
        }

        public static void JoinBeerQueue(Patron patron)
        {
            beerQueue.Add(patron);
        }
    }
}

