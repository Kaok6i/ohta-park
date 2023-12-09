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
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {

        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        DispatcherTimer timer = new DispatcherTimer();
        bool createOnlyOneWindow = true;
        bool createOnlyOneMessage = true;
        int timeLeft;
        DispatcherTimer Timer = new DispatcherTimer();
        IEnumerable<Employees_Users> EmployeeList = new ObservableCollection<Employees_Users>();
        Employees user = new Employees();
        public HistoryWindow(Employees userinfo, int continueTime)
        {
            InitializeComponent();
            timeLeft = continueTime;
            user = userinfo;
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
            // Заполнение ListView элементами из Базы данных
            EntersLV.ItemsSource = ohtaent.Employees_Users.ToList();
            // Заполнение отдельного списка элементами из Базы Данных,
            // это необохдимо для работы фильтра
            EmployeeList = ohtaent.Employees_Users.ToList();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            // Если таймер сеанса истекает, то открываем окно логина
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
            if (timeLeft == 300)
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
            var adminWndw = new AdministratorWindow(user , timeLeft, false);
            adminWndw.Show();
            createOnlyOneWindow = true;
            this.Close();
        }
        public void FilterList()
        {
            // Убираем пробелы из тексат, по которому мы будем искать в базе данных нужные нам данные
            var FilterText = FilterTB.Text.Trim();
            if (FilterText.Length > 0)
            {
                // Смотрим, какой параметр пользователь выбрал (т.е от новых к старым, или наоборот)
                if (DateCB.SelectedIndex == 0)
                {
                    EmployeeList = ohtaent.Employees_Users.OrderBy(x => x.Last_Enter).Where(x => x.Login.ToLower().Contains(FilterText.ToLower())).ToList();
                }
                else
                {
                    EmployeeList = ohtaent.Employees_Users.OrderByDescending(x => x.Last_Enter).Where(x => x.Login.ToLower().Contains(FilterText.ToLower())).ToList();
                }
            }
            else
            {
                // Если фильтрационный текст вообще не был написан, то ищем данные чисто по параметрам
                if (DateCB.SelectedIndex == 0)
                {
                    EmployeeList = ohtaent.Employees_Users.OrderBy(x => x.Last_Enter).ToList();
                }
                else
                {
                    EmployeeList = ohtaent.Employees_Users.OrderByDescending(x => x.Last_Enter).ToList();
                }
            }
            // Конечный сфильтрованный вариант присваиваем в качестве источника ListView'y
            EntersLV.ItemsSource = EmployeeList;
        }
        private void FilterTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterList();
        }
        private void DateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterTB.Text.Length <= 0)
            {
                if (DateCB.SelectedIndex == 0)
                {
                    EntersLV.ItemsSource = EmployeeList.OrderBy(x => x.Last_Enter);
                }
                else
                {
                    EntersLV.ItemsSource = EmployeeList.OrderByDescending(x => x.Last_Enter);
                }
            }
            else
            {
                FilterList();
            }  
        }       
    }
}
