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

namespace Lab6_Pub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int TIMESCALE = 1;
        public const int MAX_OPENTIME = 120;
        public const int MAX_GLASSES = 8;
        public const int MAX_CHAIRS = 9;
        public int openTime;
        public static bool open = false;
        public Timer timer;
        public static Bouncer bouncer = new Bouncer();
        public List<Patron> patrons = new List<Patron>();

        public MainWindow()
        {
            InitializeComponent();
            int glasses = MAX_GLASSES;
            int chairs = MAX_CHAIRS;
            openTime = MAX_OPENTIME;

            timer = new Timer(1000d);
            timer.Elapsed += Timer_Elapsed;

            btnPauseBartender.Click += BtnPauseBartender_Click;
            btnPauseWaitress.Click += BtnPauseWaitress_Click;
            btnPausePatrons.Click += BtnPausePatrons_Click;
            btnOpenClose.Click += BtnOpenClose_Click;
            lbPatrons.ItemsSource = patrons;
            lbPatrons.DisplayMemberPath = "PatronName";

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Patron patron = bouncer.CreatePatron();
                if (patron != null)
                {
                    patrons.Add(patron);
                    lbPatrons.Items.Refresh();
                }
                
                

                lblOpen.Content = "Closes in: " + openTime;
                openTime -= 1;
            });
            
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
            timer.Stop();
        }

        private void OpenBar()
        {
            // STARTA THREADS 
            open = true;
            timer.Start();
        }

        private void BtnPauseBartender_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
