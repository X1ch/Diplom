using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Обработка нажатия Enter
            txtLogin.KeyDown += TxtLogin_KeyDown;
            txtPassword.KeyDown += TxtPassword_KeyDown;
        }

        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(null, null);
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(login))
            {
                txtError.Text = "Введите логин!";
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите пароль!";
                txtPassword.Focus();
                return;
            }

            try
            {
                // Поиск пользователя в БД
                var user = Connect.context.Users
                    .FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    txtError.Text = "Пользователь с таким логином не найден!";
                    txtLogin.Clear();
                    txtPassword.Clear();
                    txtLogin.Focus();
                    return;
                }

                // Проверка пароля (прямое сравнение)
                if (user.PasswordHash != password)
                {
                    txtError.Text = "Неверный пароль!";
                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                // Получение роли пользователя
                var role = Connect.context.Roles
                    .FirstOrDefault(r => r.ID_Role == user.ID_Role);

                if (role == null)
                {
                    txtError.Text = "Ошибка: роль пользователя не определена!";
                    return;
                }

                // Сохранение данных в сессию
                UserSession.CurrentUserID = user.ID_Sotrudnika;
                UserSession.CurrentUserFIO = user.Fio;
                UserSession.CurrentUserLogin = user.Login;
                UserSession.CurrentUserRoleID = user.ID_Role;
                UserSession.CurrentUserRoleName = role.Name_Role;

                // Открываем главное окно
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                // Закрываем окно авторизации
                this.Close();
            }
            catch (Exception ex)
            {
                txtError.Text = $"Ошибка подключения к базе данных: {ex.Message}";
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}