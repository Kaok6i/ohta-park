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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OhtaApp
{
    /// <summary>
    /// Логика взаимодействия для CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        CapthaGenerator Captha = new CapthaGenerator();
        DispatcherTimer Timer = new DispatcherTimer();
        int error = 0;
        int sec = 10;
        public CaptchaWindow()
        {
            InitializeComponent();
            Captcha.Source = Captha.CreateImageHard(800, 800);

        }

        private void refreshBttn_Click(object sender, RoutedEventArgs e)
        {
            Captcha.Source = Captha.CreateImageHard(800, 800);
        }
        public void timer()
        {
            countDownTB.Visibility = Visibility.Visible;
            Timer.Interval = new TimeSpan(0, 0, 0, 1);
            Timer.Tick += DispatcherTimer_Tick;
            Timer.Start();
        }
        void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            if (sec > 0)
            {
                sec--;
                countDownTB.Text = String.Format("0{0}:{1}", sec / 60, sec % 60);
            }
            else
            {
                Timer.Tick -= DispatcherTimer_Tick;
                Timer.Stop();
                ImageBox.IsEnabled = true;
                refreshBttn.IsEnabled = true;
                checkBttn.IsEnabled = true;
                countDownTB.Visibility = Visibility.Hidden;                
                error = 0;
            }
        }
        private void checkBttn_Click(object sender, RoutedEventArgs e)
        {
            if (Captha.CapthaChecker(ImageBox.Text))
            {
                MessageBox.Show("Вы прошли капчу!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                var mw = new LoginWindow(true);
                mw.Show();
                this.Close();
            }
            else if (error == 1)
            {
                MessageBox.Show("Вы не прошли капчу!\nПопробуйте ещё раз через 10 секунд.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                ImageBox.IsEnabled = false;
                refreshBttn.IsEnabled = false;
                checkBttn.IsEnabled = false;
                sec = 10;
                timer();
            }
            else
            {
                error++;
                MessageBox.Show("Вы не прошли капчу!\nУ вас ещё одна попытка.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

