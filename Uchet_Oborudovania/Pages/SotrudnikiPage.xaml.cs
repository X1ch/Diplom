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
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania.Pages
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            CheckUser();
            UsersDG.ItemsSource = Connect.context.Users.ToList();
            combobox1.ItemsSource = Connect.context.Users.Select(x => x.Dolzhnost).ToList().Distinct().ToList();
        }

        public void CheckUser()
        {
            string role = UserSession.CurrentUserRoleName;
            switch (role)
            {
                case "Администратор":
                    AddBtn.Visibility = Visibility.Visible;
                    LoginCB.Visibility = Visibility.Visible;
                    PasswordCB.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new AddEditUsers(null));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UsersDG.ItemsSource = Connect.context.Users.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new AddEditUsers((sender as Button).DataContext as Users));
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            var delemployee = UsersDG.SelectedItems.Cast<Users>().ToList();
            foreach (var delemployes in delemployee)
                if (Connect.context.Vidachi.Any(x => x.ID_Sotrudnika == delemployes.ID_Sotrudnika))
                {
                    MessageBox.Show("Данные используются в другой таблице", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            if (MessageBox.Show($"Удалить{delemployee.Count} записей", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Connect.context.Users.RemoveRange(delemployee);
            try
            {
                Connect.context.SaveChanges();
                UsersDG.ItemsSource = Connect.context.Users.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.GoBack();
        }

        private void SortBtn_Click(object sender, RoutedEventArgs e)
        {
            var sortedEmployees = Connect.context.Users.OrderBy(r => r.Fio).ToList();
            UsersDG.ItemsSource = sortedEmployees;
        }

        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            var filterEmployee = combobox1.SelectedItem.ToString();
            var employees = Connect.context.Users.ToList();
            var filteredEmployees = employees.Where(r => r.Dolzhnost == filterEmployee).ToList();
            UsersDG.ItemsSource = filteredEmployees;
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsersDG.ItemsSource = Connect.context.Users.ToList().Where(r => r.Fio.ToLower().Contains(searchTB.Text.ToString().ToLower())).ToList();

        }
    }
}
