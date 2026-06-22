using System;
using System.Windows;
using System.Windows.Controls;
using Uchet_Oborudovania.AppData;
using Uchet_Oborudovania.Pages;
using System.Linq;

namespace Uchet_Oborudovania.Pages
{

    public partial class AddEditUsers : Page
    {
        Users User;
        bool checkNew;
        public AddEditUsers(Users c)
        {
            InitializeComponent();
            CheckUser();
            cbrole.ItemsSource = Connect.context.Roles.ToList();
            if (c == null)
            {
                c = new Users();
                checkNew = true;
            }
            else
                checkNew = false;
            DataContext = User = c;
            

        }

        public void CheckUser()
        {
            string role = UserSession.CurrentUserRoleName;
            switch (role)
            {
                case "Администратор":
                    LoginTB.Visibility = Visibility.Visible;
                    LoginTXT.Visibility = Visibility.Visible;
                    PasswordTB.Visibility = Visibility.Visible;
                    PasswordTXT.Visibility = Visibility.Visible;
                    break;
            }
        }

           

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (checkNew)
            {
                Connect.context.Users.Add(User);
            }
            try
            {
                Connect.context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Nav.MainFrame.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.GoBack();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
