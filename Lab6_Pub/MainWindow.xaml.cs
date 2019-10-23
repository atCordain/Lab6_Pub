using System;
using System.Windows;
using System.Windows.Threading;

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
            
            btnPausePatrons.Click += BtnPausePatrons_Click;
            btnOpenClose.Click += BtnOpenClose_Click;
            btnStopAll.Click += BtnStopAll_Click;
            btnPauseBartender.Click += BtnPauseBartender_Click;
            btnPauseWaitress.Click += BtnPauseWaitress_Click;

            //btnIncreaseSpeed.Click += IncreaseSpeed_Click;

            Bouncer.LogThis += Bouncer_LogThis;
            Patron.LogThis += Patron_LogThis;
            Bartender.LogThis += Bartender_LogThis;
            Waitress.LogThis += Waitress_LogThis;
            bar = new Bar();
            bar.OpenBar();
            bar.timer.Elapsed += Timer_Elapsed;
            UpdateStatusLabels();
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
            Dispatcher.Invoke(() => lblOpen.Content = $"Closes in: {Bar.OpenTimeLeft}");
        }

        private void UpdateStatusLabels()
        {
            Dispatcher.Invoke(() => lblGlasses.Content = $"There are {Bar.AvailableGlasses} free Glasses ({Bar.TotalGlassesInBar} total)");
            Dispatcher.Invoke(() => lblPatrons.Content = $"There are {Bar.PatronsInBar} Patrons in the bar");
            Dispatcher.Invoke(() => lblTables.Content = $"There are {Bar.AvailableTables} free Tables ({Bar.TotalTablesInBar} total)");
        }

        private void Waitress_LogThis(object sender, EventArgs e)
        {
            InsertInWaitressListbox((e as EventMessage).Message);
        }

        private void Bartender_LogThis(object sender, EventArgs e)
        {
            InsertInBartenderListbox((e as EventMessage).Message);
        }

        private void Patron_LogThis(object sender, EventArgs e)
        {
            InsertInPatronListbox((e as EventMessage).Message);
        }

        private void Bouncer_LogThis(object sender, EventArgs e)
        {
            InsertInPatronListbox((e as EventMessage).Message);
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
            if (Bar.IsBarOpen) Bar.OpenTimeLeft = 1;
            else bar.OpenBar();
        }

        public void InsertInPatronListbox(string text)
        {
            Dispatcher.Invoke(() => lbPatrons.Items.Insert(0, $"{logIndex++} - {text}"));
            UpdateStatusLabels();
        }

        public void InsertInWaitressListbox(string text)
        {
            Dispatcher.Invoke(() => lbWaitress.Items.Insert(0, $"{logIndex++} - {text}"));
            UpdateStatusLabels();
        }

        public void InsertInBartenderListbox(string text)
        {
            Dispatcher.Invoke(() => lbBartender.Items.Insert(0, $"{logIndex++} - {text}"));
            UpdateStatusLabels();
        }
    }
}

