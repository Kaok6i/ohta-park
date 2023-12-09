using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Net.Mime.MediaTypeNames;

namespace OhtaApp
{
    /// <summary>
    /// Логика взаимодействия для ServicesWindow.xaml
    /// </summary>
    public partial class ServicesWindow : Window
    {

        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        Employees user = new Employees();
        Clients client = new Clients();
        public ServicesWindow(Employees userinfo, int continueTime, Clients clientinfo)
        {
            InitializeComponent();
            timeLeft = continueTime;
            user = userinfo;
            client = clientinfo;
        }

        DispatcherTimer timer = new DispatcherTimer();
        bool createOnlyOneWindow = true;
        bool createOnlyOneMessage = true;
        DispatcherTimer Timer = new DispatcherTimer();
        int timeLeft;
        IEnumerable<Services> ServicesList = new ObservableCollection<Services>();
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            var createorder = new CreatingOrderWindow(user, timeLeft);
            createorder.Show();
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
            ServicesDG.ItemsSource = ohtaent.Services.ToList();
            ServicesList = ohtaent.Services.ToList();
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
            countDownTB.Text = String.Format("Время сеанса: " + "{0}:{1}", timeLeft / 60, timeLeft % 60);
            if (timeLeft <= 0)
            {
                timeLeft = 60;
                Timer.Stop();
            }
            timeLeft--;
        }
        // Список выбранных услуг
        List<int> ServiceIDList = new List<int>();
        private void addServiceBttn_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, была ли выбрана строка услуги
            if (ServicesDG.SelectedItem == null)
            {
                MessageBox.Show("Вы не выбрали услугу для добавления в заказ!", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Проверка, не пустая ли строка услуги
                if (ServicesDG.SelectedItem != null)
                {
                    // Выбираем данные об услуги из Базы Данных
                    Services service = ServicesDG.SelectedItem as Services;
                    // Смотрим, есть ли эта услуга уже в списке
                    if (ServiceIDList.Contains(service.ID) == false)
                    {  // Если нету, то добавляем в список услуг
                        ServiceIDList.Add(service.ID);
                        // Графическое отображение добавленной услуги
                        servicesTBl.Text += string.Format(" \n{0};", service.Name);
                        ServicesDG.SelectedItem = null;
                    }
                    else
                    {
                        MessageBox.Show("Нельзя добавить одну и ту же услугу дважды!", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void addOrderBttn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, были ли добавлены услуги к заказу
            if (servicesTBl.Text != null && servicesTBl.Text != "" && ServiceIDList.Count > 0)
            { // Проверяем, было ли вписано время проката
                if (RentalTB.Text != null && RentalTB.Text != "")
                {
                    // Если все проверки успешно пройдены,то создаем новый заказ в Базе Данных
                    Orders order = new Orders();
                    order.ID = ohtaent.Orders.Max(x => x.ID) + 1;
                    order.Code = string.Format("{0}/{1}", client.Code, DateTime.Now.ToString("yyyy-MM-dd"));
                    order.Creation_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                    order.Creation_Time = DateTime.Now.TimeOfDay;
                    order.Client_Code = client.Code;
                    order.ID_Status = 1;
                    order.Rental_Time_In_Minutes = Convert.ToInt32(RentalTB.Text);
                    ohtaent.Orders.Add(order);
                    ohtaent.SaveChanges();
                    for (int i = 0; i < ServiceIDList.Count; i++)
                    {
                        Services_Orders orders = new Services_Orders();
                        orders.ID = ohtaent.Services_Orders.Max(x => x.ID) + 1;
                        orders.ID_Order = order.ID;
                        orders.ID_Service = ServiceIDList[i];
                        ohtaent.Services_Orders.Add(orders);
                        ohtaent.SaveChanges();
                    }
                    // Уведомляем пользователя об успешности операций
                    MessageBox.Show("Заказ успешно оформлен!", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Возращаем пользователя обратно на окно с выбором клиентов
                    backButton_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Вы забыли вписать время проката!", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали услуги для клиента!", "Охта Парк", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void FilterList()
        {
            var FilterText = FilterTB.Text.Trim();
            if (FilterText.Length > 0)
            {
                ServicesList = ohtaent.Services.Where(x => x.Name.ToLower().Contains(FilterText.ToLower())).ToList();
            }
            ServicesDG.ItemsSource = ServicesList;
        }
        private void FilterTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterList();
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void RentalTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
           e.Handled = !IsTextAllowed(e.Text);
        }
    }
}

