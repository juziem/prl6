using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Media;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;

namespace lab6
{
    //new
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer player = new MediaPlayer();
        string tmp;
        bool go;

        System.Windows.Threading.DispatcherTimer Timer;
        TimeSpan ts;

        Dictionary<string, string> sound = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();

            player.MediaOpened += Player_MediaOpened;

            Timer = new System.Windows.Threading.DispatcherTimer();
            Timer.Tick += new EventHandler(dispatcherTimer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            try
            {
                slide.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;//длительность содержимого
                slide.Value = 0;
            }
            catch
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Uncorrect format");
            }
            //head.Content = "0:0:0";
            //tail.Content = player.NaturalDuration.TimeSpan.Hours + ":" + player.NaturalDuration.TimeSpan.Minutes + ":" + player.NaturalDuration.TimeSpan.Seconds;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(go==false)
            {
                head.Content= player.Position.Hours + ":" + player.Position.Minutes + ":" + player.Position.Seconds;
                slide.Value= player.Position.TotalSeconds;
            }
        }

            private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
            tmp = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);

            sound.Add(tmp, dlg.FileName);
            list.Items.Add(tmp);
        }     

        private void Play_Click(object sender, RoutedEventArgs e)
        {
                if (list.SelectedIndex > -1)//если элемент выбран
                {
                    player.Play();
                    Timer.Start();

                    head.Content = "0:0:0";
                    tail.Content = player.NaturalDuration.TimeSpan.Hours + ":" + player.NaturalDuration.TimeSpan.Minutes + ":" + player.NaturalDuration.TimeSpan.Seconds;
                }
                else
            {               
                Random rnd = new Random();
                int num = list.Items.Count;               

                num = rnd.Next(0, num - 1);

                tmp = list.Items[num].ToString();
                string melody = sound[tmp];
                player.Open(new Uri(melody, UriKind.Relative));
                //Thread.Sleep(800);
                player.Play();
                Timer.Start();
                list.SelectedItem = tmp;

                //не понимаю, в чем прикол с head/tail...
                head.Content = "0:0:0";
                tail.Content = player.NaturalDuration.TimeSpan.Hours + ":" + player.NaturalDuration.TimeSpan.Minutes + ":" + player.NaturalDuration.TimeSpan.Seconds;
               
            }

        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tmp = list.Items[list.SelectedIndex].ToString();
                string corr = sound[tmp];
                player.Open(new Uri(corr, UriKind.Relative));
            }
            catch
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Choose melodies!");
            }
        }

        private void Slide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)slide.Value;
            head.Content = (sliderValue / 3600).ToString() + ":" + (sliderValue / 60).ToString() + ":" + (sliderValue % 60).ToString();


            //ts = new TimeSpan(0, 0, sliderValue);

            //player.Position = ts;
            //go = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            go = true;
        }

        private void Slider_DragEnded(object sender, DragCompletedEventArgs e)
        {
            int sliderValue = (int)slide.Value;

            ts = new TimeSpan(0, 0, sliderValue);
            player.Position = ts;
            go = false;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Timer.Stop();
            head.Content = "";
            tail.Content = "";
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double sliderValue = (double)volume.Value;
            player.Volume = sliderValue;
        }
    }
}
