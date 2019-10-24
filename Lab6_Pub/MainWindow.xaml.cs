using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static Lab6_Pub.Bar;

namespace Lab6_Pub
{
    public partial class MainWindow : Window
    {
        private Bar bar;
        private int logIndex;

        public MainWindow()
        {
            InitializeComponent();
            logIndex = 0;
            
            cbStartCondition.ItemsSource = Enum.GetNames(typeof(StartCondition));
            
            btnPausePatrons.Click += BtnPausePatrons_Click;
            btnOpenClose.Click += BtnOpenClose_Click;
            btnStopAll.Click += BtnStopAll_Click;
            btnPauseBartender.Click += BtnPauseBartender_Click;
            btnPauseWaitress.Click += BtnPauseWaitress_Click;
            btnIncreaseSpeed.Click += IncreaseSpeed_Click;
            cbStartCondition.SelectionChanged += Set_StartCondition;

            Bouncer.LogThis += Bouncer_LogThis;
            Patron.LogThis += Patron_LogThis;
            Bartender.LogThis += Bartender_LogThis;
            Waitress.LogThis += Waitress_LogThis;
            
            bar = new Bar();
            
            UpdateStatusLabels();
        }

        private void Set_StartCondition(object sender, SelectionChangedEventArgs e)
        {
            switch (cbStartCondition.SelectedIndex)
            {
                case 1:
                    bar.startCondition = StartCondition.TwentyGlassThreeChairs;
                    break;
                case 2:
                    bar.startCondition = StartCondition.TwentyChairsFiveGlass;
                    break;
                case 3:
                    bar.startCondition = StartCondition.GuestStayDoubled;
                    break;
                case 4:
                    bar.startCondition = StartCondition.WaitresWorksDoubleSpeed;
                    break;
                case 5:
                    bar.startCondition = StartCondition.BarOpenFiveMinuites;
                    break;
                case 6:
                    bar.startCondition = StartCondition.CouplesNight;
                    break;
                case 7:
                    bar.startCondition = StartCondition.PatronBusLoad;
                    break;
                default:
                    bar.startCondition = StartCondition.Standard;
                    break;
            }
            cbStartCondition.IsEnabled = false;
        }
        private void IncreaseSpeed_Click(object sender, RoutedEventArgs e)
        {
            bar.IncreaseSimulationSpeed();
        }

        private void BtnPauseWaitress_Click(object sender, RoutedEventArgs e)
        {
            bar.PauseResumeWaitress();
        }

        private void BtnPauseBartender_Click(object sender, RoutedEventArgs e)
        {
            bar.PauseResumeBartender(); 
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => lblOpen.Content = $"Closes in: {Bar.openTimeLeft}");
        }

        private void UpdateStatusLabels()
        {
            Dispatcher.Invoke(() => lblGlasses.Content = $"There are {Bar.AvailableGlasses} free Glasses ({Bar.TotalGlassesInBar} total)");
            Dispatcher.Invoke(() => lblPatrons.Content = $"There are {Bar.PatronsInBar} Patrons in the bar");
            Dispatcher.Invoke(() => lblTables.Content = $"There are {Bar.AvailableTables} free Tables ({Bar.TotalTablesInBar} total)");
        }

        private void Waitress_LogThis(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, $"{logIndex++} - {(e as EventMessage).Message}"));
            UpdateStatusLabels();
        }

        private void Bartender_LogThis(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => lbBartender.Items.Insert(0, $"{logIndex++} - {(e as EventMessage).Message}"));
            UpdateStatusLabels();
        }

        private void Patron_LogThis(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, $"{logIndex++} - {(e as EventMessage).Message}"));
            UpdateStatusLabels();
        }

        private void Bouncer_LogThis(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, $"{logIndex++} - {(e as EventMessage).Message}"));
            UpdateStatusLabels();
        }

        private void BtnStopAll_Click(object sender, RoutedEventArgs e)
        {
            bar.Cancel();
        }

        private void BtnPausePatrons_Click(object sender, RoutedEventArgs e)
        {
            bar.PauseResumeBouncerPatrons();
        }
        
        private void BtnOpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (Bar.IsBarOpen) 
            {
                Bar.openTimeLeft = 1;
                btnOpenClose.IsEnabled = false;
                btnIncreaseSpeed.IsEnabled = false;
                btnPauseBartender.IsEnabled = false;
                btnPausePatrons.IsEnabled = false;
                btnPauseWaitress.IsEnabled = false;
                btnStopAll.IsEnabled = false;
            } 
            else 
            {
                bar.OpenBar();
                bar.timer.Elapsed += Timer_Elapsed;
                cbStartCondition.IsEnabled = false;
            };
        }
    }
}
