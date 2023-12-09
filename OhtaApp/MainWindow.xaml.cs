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
using System.Windows.Threading;

namespace OhtaApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        int errorCount = 0;
        int timeLeft;
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
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
        // Событие для входа в систему
        private void loginbutton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, введено ли хоть что-то в предложенных окнах
            if (loginTB.Text != "" && passwordTB.Text != "" || passwordPB.Password != "")
            {
                // Проверяем, не было ли предыдущих неудачных попыток входа в систему
                if (errorCount < 1)
                {
                    // Проверка, сущетсвуют ли пользователь по введенным данным в Базе Данных
                    if (ohtaent.Employees_Users.Any(u => u.Login == loginTB.Text) && (ohtaent.Employees_Users.Any(u => u.Password == passwordTB.Text) 
                        || ohtaent.Employees_Users.Any(u => u.Password == passwordPB.Password)))
                    {
                        Employees_Users user = ohtaent.Employees_Users.Where(x => x.Login == loginTB.Text).First();
                        Employees employee = ohtaent.Employees.Find(user.ID_Employee);
                        // Уведомляем пользователя об успешном входе
                        MessageBox.Show("Вы успешно зашли, " + employee.Name + "!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Переходим в необходимое окно, в зависимости от роля пользователя
                        switch (employee.ID_Role)
                        {
                            case 1:
                                SellerWindow sw = new SellerWindow(employee,0,true);
                                sw.Show();
                                break;
                            case 2:
                                AdministratorWindow aw = new AdministratorWindow(employee,0,true);
                                aw.Show();
                                break;
                            case 3:
                                ShiftSupervisorWindow ssw = new ShiftSupervisorWindow(employee,0, true);
                                ssw.Show();
                                break;
                        }
                        this.Close();
                    }
                    else
                    {
                        // При неправильно логине/пароле увеличиваем счетчик невернхы входов  в систему, для того чтобы после 2 попытки пользователю нужно было вводить капчу
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
                // Уведомление пользователю о том, что он вообще ничего не ввел в предложенные поля
                MessageBox.Show("Введие логин и пароль для входа!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
    }
}

