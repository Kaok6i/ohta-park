using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AdministratorWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {

        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        DispatcherTimer timer = new DispatcherTimer();
        bool createOnlyOneWindow = true;
        bool createOnlyOneMessage = true;
        int timeLeft = 600;
        DispatcherTimer Timer = new DispatcherTimer();
        Employees user = new Employees();
        // Переменная firstload нужна для того, чтобы понимать, впервые это окно запускается или нет
        // Если впервые то таймер сеанса начнет свой счет, если окно открывается не в первый раз
        // То идет продолжение отсчета сеанса пользователя с переданного из другого окна времени
        public AdministratorWindow(Employees userinfo , int continueTime, bool firstLoad)
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
            timer.Interval = TimeSpan.FromSeconds(1); 
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Tick += new EventHandler(timerLeft_Tick);
            Timer.Start();
            // Отображение роли/должности пользователя на форме
            RoleTBl.Text = ohtaent.Roles.Where(x => x.ID == user.ID_Role).Select(x => x.Name).First().ToString();
            // Отображение имени пользователя
            NameTBl.Text = user.Name;
            // Отображение фамилий пользователя
            SurnameTBl.Text = user.Surname;
            // Отображение изображения пользователя на форме
            AvatarImg.Source = ConvertImage.LoadImage(user.Image);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                timer.Stop();
                if (createOnlyOneWindow)
                {
                    // Указываем в параметре, что сеанс завершился автоматически, и нужно открыть окно логина таким образом,
                    // чтобы пользователь не мог входить в систему в течений 3 минут
                    var mw = new LoginWindow(false);
                    mw.Show();
                    createOnlyOneWindow = false;
                }
                this.Close();
            }
            // 300 секунд = 5 минут
            if (timeLeft <= 300)
            {
                // Переменная, которая позволяет вывести сообщение, о том что сеанс закончится через 5 минут один раз
                // Т.к если её не будет, то пользователю постоянно будет сыпать уведомление
                if (createOnlyOneMessage)
                {
                    MessageBox.Show("Сеанс закончится через 5 минут!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    createOnlyOneMessage = false;
                }
            }

        }
        // Событие, в котором отображается оставшееся время сеанса
        public void timerLeft_Tick(object sender, EventArgs e)
        {
            countDownTB.Text = String.Format("Время сеанса: "+"{0}:{1}", timeLeft / 60, timeLeft % 60);
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
            // Указываем в парамтре, что пользователь сам захотел вернуться в окно логина, и
            // соотвественно не нужно блокировать вход в систему на 3 минуты
            var mw = new LoginWindow(true);
            mw.Show();
            createOnlyOneWindow = true;
            this.Close();
        }
        // Событие для перехода в окно просмотра историй входа
        private void HistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            // Передаем информацию об текущем пользователе, и его оставшееся время сеанса
            var history = new HistoryWindow(user, timeLeft);
            history.Show();
            this.Close();
        }
    }
}

