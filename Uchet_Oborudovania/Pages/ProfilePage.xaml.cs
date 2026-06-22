using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania.Pages
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                // Загружаем данные из сессии
                TxtFIO.Text = UserSession.CurrentUserFIO;
                TxtLogin.Text = UserSession.CurrentUserLogin;
                TxtRole.Text = UserSession.CurrentUserRoleName;

                // Дополнительно загружаем должность из БД (если есть)
                var user = Connect.context.Users
                    .FirstOrDefault(u => u.ID_Sotrudnika == UserSession.CurrentUserID);

                if (user != null)
                {
                    TxtDolzhnost.Text = user.Dolzhnost ?? "Не указана";
                }
                else
                {
                    TxtDolzhnost.Text = "Не указана";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных профиля: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Устанавливаем значения по умолчанию
                TxtFIO.Text = UserSession.CurrentUserFIO ?? "Не указано";
                TxtLogin.Text = UserSession.CurrentUserLogin ?? "Не указан";
                TxtRole.Text = UserSession.CurrentUserRoleName ?? "Не указана";
                TxtDolzhnost.Text = "Не указана";
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            // Возврат на предыдущую страницу
            if (Nav.MainFrame.CanGoBack)
            {
                Nav.MainFrame.GoBack();
            }
            else
            {
                // Если некуда возвращаться, идем на страницу ремонта
                Nav.MainFrame.Navigate(new RemontiPage());
            }
        }
    }
}