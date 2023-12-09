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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        int errorCount = 0;
        int timeLeft = 180;
        bool manualEnter;
        DispatcherTimer timer = new DispatcherTimer();
        public LoginWindow(bool enter)
        {
            InitializeComponent();
            manualEnter = enter;
            passwordCloseButton.Visibility = Visibility.Hidden;
        }
        // Событие для открытия пароля
        private void passwordOpenButton_Click(object sender, RoutedEventArgs e)
        {
            passwordOpenButton.Visibility = Visibility.Hidden;
            passwordCloseButton.Visibility = Visibility.Visible;
            passwordTB.Text = passwordPB.Password;
            passwordTB.Visibility = Visibility.Visible;
            passwordPB.Visibility = Visibility.Hidden;
        }
        // Событие для закрытия пароля
        private void passwordCloseButton_Click(object sender, RoutedEventArgs e)
        {
            passwordOpenButton.Visibility = Visibility.Visible;
            passwordCloseButton.Visibility = Visibility.Hidden;
            passwordPB.Password = passwordTB.Text;
            passwordTB.Visibility = Visibility.Hidden;
            passwordPB.Visibility = Visibility.Visible;
        }
        private void loginbutton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, введено ли хоть что-то в предложенных окнах
            if (loginTB.Text != "" && passwordTB.Text != "" || passwordPB.Password != "")
            {
                // Проверяем, не было ли предыдущих неудачных попыток входа в систему
                if (errorCount < 1)
                {
                    if (ohtaent.Employees_Users.Any(u => u.Login == loginTB.Text) && (ohtaent.Employees_Users.Any(u => u.Password == passwordTB.Text) || ohtaent.Employees_Users.Any(u => u.Password == passwordPB.Password)))//проверка логина и пароля на существование в бд
                    {
                        Employees_Users user = ohtaent.Employees_Users.Where(x => x.Login == loginTB.Text).First();
                        Employees employee = ohtaent.Employees.Find(user.ID_Employee);
                        // Уведомляем пользователя об успешном входе
                        MessageBox.Show("Вы успешно зашли, " + employee.Name+"!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Переходим в необходимое окно, в зависимости от роля пользователя
                        switch (employee.ID_Role)
                        {
                            case 1:
                                SellerWindow sw = new SellerWindow(employee, 600, true);
                                sw.Show();
                                break;
                            case 2:
                                AdministratorWindow aw = new AdministratorWindow(employee, 600,true);
                                aw.Show();
                                break;
                            case 3:
                                ShiftSupervisorWindow ssw = new ShiftSupervisorWindow(employee, 600, true);
                                ssw.Show();
                                break;
                        }
                        this.Close();
                    }
                    else
                    {
                        errorCount++;
                        MessageBox.Show("Не удалось войти в систему", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Если до этого был неудачная попытка входа, то открывается окно с капчей
                    CaptchaWindow cptchWndw = new CaptchaWindow();
                    cptchWndw.Show();
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show("Введие логин и пароль для входа!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверяем, окно логина открыл сам пользователь, или его сюда выбросило после истечения времени сеанса
            if (!manualEnter)
            {
                timeLeftToEnterTB.Visibility = Visibility.Visible;
                loginButton.IsEnabled = false;
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            // Если пользователь попал в окно логина по истечению сеанса, то после блокирования входа показываем, через какое
            // время он сможет снова зайти в систему
            timeLeftToEnterTB.Text = string.Format("Повторный вход через: " + "{0}:{1}", timeLeft / 60, timeLeft % 60);
            if (timeLeft <= 0)
            {
                timeLeftToEnterTB.Visibility = Visibility.Hidden;
                loginButton.IsEnabled = true;
                timer.Stop();
            }
            timeLeft--;
        }
    }
}
