using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        bool createOnlyOneWindow = true;
        bool createOnlyOneMessage = true;
        DispatcherTimer Timer = new DispatcherTimer();
        int timeLeft;
        Employees user = new Employees();
        public AddClientWindow(Employees userinfo,int continuetime)
        {
            InitializeComponent();
             timeLeft = continuetime;
             user = userinfo;
        }
        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        private void completeButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что строки с данными клиента не пустые
            if (nameTB.Text != "" && surnameTB.Text != "" && birthdayDP.Text != "" && addressTB.Text != "" && emailTB.Text != "" && passwordTB.Text != "")
            {
                // Создание нового клиента
                Clients client = new Clients
                {
                    // Вводим введенные данные в Базу Данных 
                    Code = codeTB.Text,
                    Name = nameTB.Text,
                    Surname = surnameTB.Text,
                    Patronymic = patronymicTB.Text,
                    Birthday = DateTime.Parse(birthdayDP.DisplayDate.ToShortDateString()),
                    Passport_Series = passportSeriesTB.Text,
                    Passport_Number = passportNumberTB.Text,
                    Address = addressTB.Text,
                    Email = emailTB.Text,
                    Password = passwordTB.Text,
                };
                ohtaent.Clients.Add(client);
                ohtaent.SaveChanges();
                MessageBox.Show("Данные были успешно добавлены!", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Information);
                backButton_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Заполните все  необохдимые поля! \nЕдинственное необязательное поле - Отчество", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            int continueTime = timeLeft;
            var createOrder = new CreatingOrderWindow(user,continueTime);
            createOrder.Show();
            this.Close();
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
            countDownTB.Text = String.Format("Время сеанса: "+"{0}:{1}", timeLeft / 60, timeLeft % 60);
            if (timeLeft <= 0)
            {
                timeLeft = 60;
                Timer.Stop();
            }
            timeLeft--;
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void pasportSeriesTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void pasportNumberTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
