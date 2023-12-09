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
    /// Логика взаимодействия для SellerWindow.xaml
    /// </summary>
    public partial class SellerWindow : Window
    {

        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        DispatcherTimer timer = new DispatcherTimer();
        bool createOnlyOneWindow = true;
        bool createOnlyOneMessage = true;
        int timeLeft = 600;
        DispatcherTimer Timer = new DispatcherTimer();
        Employees user = new Employees();
        public SellerWindow(Employees userinfo, int continueTime, bool firstLoad)
        {
            InitializeComponent();
            user = userinfo;
            if (!firstLoad)
            {
                timeLeft = continueTime;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // интервал времени в секундах
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            Timer.Interval = TimeSpan.FromMilliseconds(1000); // интервал времени в миллисекундах
            Timer.Tick += new EventHandler(timerLeft_Tick);
            Timer.Start();
            RoleTBl.Text = ohtaent.Roles.Where(x => x.ID == user.ID_Role).Select(x => x.Name).First().ToString();
            NameTBl.Text = user.Name;
            SurnameTBl.Text = user.Surname;
            AvatarImg.Source = ConvertImage.LoadImage(user.Image);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                timer.Stop();
                if (createOnlyOneWindow)
                {
                    var mw = new LoginWindow(false);
                    mw.Show();
                    createOnlyOneWindow = false;
                }
                this.Close();
            }
            if (timeLeft == 300)
            {
                if (createOnlyOneMessage)
                {
                    MessageBox.Show("Сеанс закончится через 5 минут!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    createOnlyOneMessage = false;
                }
            }

        }
        public void timerLeft_Tick(object sender, EventArgs e)
        {
            countDownTB.Text = String.Format("Время сеанса:"+"{0}:{1}", timeLeft / 60, timeLeft % 60);
            if (timeLeft <= 0)
            {
                timeLeft = 60;
                Timer.Stop();
            }
            timeLeft--;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            var mw = new LoginWindow(true);
            mw.Show();
            createOnlyOneWindow = true;
            this.Close();
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            var createorder = new CreatingOrderWindow(user, timeLeft);
            createorder.Show();
            this.Close();
        }
    }
}
