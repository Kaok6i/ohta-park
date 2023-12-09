using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для CreatingOrderWindow.xaml
    /// </summary>
    public partial class CreatingOrderWindow : Window
    {
        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        DispatcherTimer timer = new DispatcherTimer();
        bool createOnlyOneWindow = true;
        bool createOnlyOneMessage = true;
        int timeLeft;
        DispatcherTimer Timer = new DispatcherTimer();
        Employees user = new Employees();
        IEnumerable<Clients> ClientsList = new ObservableCollection<Clients>();
        public CreatingOrderWindow(Employees userinfo, int continueTime)
        {
            InitializeComponent();
            user = userinfo;
            timeLeft= continueTime;
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
            ClientsDB.ItemsSource = ohtaent.Clients.ToList();
            ClientsList = ohtaent.Clients.ToList();
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

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            switch (user.ID_Role)
            {
                case 1:
                    var sw = new SellerWindow(user, timeLeft, false);
                    sw.Show();
                    break;
                case 3:
                    var ssw = new ShiftSupervisorWindow(user, timeLeft, false);
                    ssw.Show();
                    break;
            }
            this.Close();
        }

        private void addClientBttn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            var addclient = new AddClientWindow(user,timeLeft);
            addclient.Show();
            this.Close();
        }
        
        private void chooseClientBttn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, была ли выбрана строчка с данными клиента
            if (ClientsDB.SelectedItem == null)
            {
                MessageBox.Show("Вы не выбрали запись для изменения!", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                // Проверяем, что строка не пустая
                if (ClientsDB.SelectedItem != null)
                {
                    // Ищем данные о клиенте в Базе Данных
                    Clients client = ClientsDB.SelectedItem as Clients;
                    timer.Stop();
                    Timer.Stop();
                    // А затем эти данные передаем в окно создания заказа
                    var service = new ServicesWindow(user, timeLeft, client);
                    service.Show();
                    this.Close();
                }
            }
            
        }
        public void FilterList()
        {
            var FilterText = FilterTB.Text.Trim();
            if (FilterText.Length > 0)
            {               
                    ClientsList = ohtaent.Clients.Where(x => x.Email.ToLower().Contains(FilterText.ToLower())).ToList();
            }
            ClientsDB.ItemsSource = ClientsList;
        }
        private void FilterTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterList();
        }
    }
}
