using System.Windows;
using System.Windows.Controls;
using Uchet_Oborudovania.AppData;
using Uchet_Oborudovania.Pages;

namespace Uchet_Oborudovania
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Nav.MainFrame = MainContentFrame;

            // Загружаем данные пользователя
            LoadUserProfile();

            // Применяем ограничения по ролям
            ApplyRoleRestrictions();

            // Открываем страницу по умолчанию
            BtnRepairs_Click(null, null);
        }

        private void LoadUserProfile()
        {
            // Заполняем информацию о пользователе в верхней панели
            TxtUserFIO.Text = UserSession.CurrentUserFIO;
            TxtUserRole.Text = UserSession.CurrentUserRoleName;
        }



        private void ApplyRoleRestrictions()
        {
            string role = UserSession.CurrentUserRoleName;

            // Скрываем кнопки по умолчанию
            BtnSpisanie.Visibility = Visibility.Collapsed;
            BtnEquipment.Visibility = Visibility.Collapsed;
            BtnSklad.Visibility = Visibility.Collapsed;
            BtnEmployees.Visibility = Visibility.Collapsed;
            BtnReports.Visibility = Visibility.Collapsed;
            BtnIssues.Visibility = Visibility.Collapsed;

            switch (role)
            {
                case "Администратор":
                    BtnEmployees.Visibility = Visibility.Visible;
                    BtnReports.Visibility = Visibility.Visible;
                    BtnIssues.Visibility = Visibility.Visible;
                    BtnSklad.Visibility = Visibility.Visible;
                    BtnSpisanie.Visibility = Visibility.Visible;
                    BtnEquipment.Visibility = Visibility.Visible;
                    break;
                case "Директор":
                    BtnEmployees.Visibility = Visibility.Visible;
                    BtnEquipment.Visibility = Visibility.Visible;
                    BtnReports.Visibility = Visibility.Visible;
                    BtnIssues.Visibility = Visibility.Visible;
                    BtnSklad.Visibility = Visibility.Visible;
                    BtnSpisanie.Visibility = Visibility.Visible;
                    break;
                case "Техник":
                    BtnSpisanie.Visibility = Visibility.Visible;
                    BtnIssues.Visibility = Visibility.Visible;
                    BtnReports.Visibility = Visibility.Visible;
                    BtnSklad.Visibility = Visibility.Visible;

                    break;
                case "Сотрудник":
                    // Только оборудование и ремонт
                    break;
            }
        }

        private void UpdatePageTitle(string title)
        {
            PageTitle.Text = title;
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("Доступ запрещен! Только для администратора.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Nav.MainFrame.Navigate(new UsersPage());
            UpdatePageTitle("👥 Сотрудники");
        }

        private void BtnEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsTechnician)
            {
                MessageBox.Show("Техник не имеет доступа к оборудованию.", "Ограничение доступа",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Nav.MainFrame.Navigate(new OborudovaniePage());
            UpdatePageTitle("🖥️ Оборудование");
        }

        private void BtnRepairs_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new RemontiPage());
            UpdatePageTitle("🔧 Ремонт");
        }

        private void BtnIssues_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsTechnician && !UserSession.IsDirector && !UserSession.IsAdmin)
            {
                MessageBox.Show("Доступ к выдаче оборудования есть только у Техника, Директора и Администратора.",
                    "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Nav.MainFrame.Navigate(new VidachiPage());
            UpdatePageTitle("📤 Выдача");
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsDirector && !UserSession.IsTechnician && !UserSession.IsAdmin)
            {
                MessageBox.Show("Доступ к отчетам есть только у Директора, Техника и Администратора.",
                    "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Nav.MainFrame.Navigate(new ReportsPage());
            UpdatePageTitle("📊 Отчеты");
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Clear();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            this.Close();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new ProfilePage());
            UpdatePageTitle("👤 Профиль пользователя");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Не используется
        }

        private void BtnSklad_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new SkladPage());
            UpdatePageTitle("📦 Склад");
        }

        private void BtnSpisanie_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new SpisaniePage());
            UpdatePageTitle("🗑 Списание");
        }
    }
}