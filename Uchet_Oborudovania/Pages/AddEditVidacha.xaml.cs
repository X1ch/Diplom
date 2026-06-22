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
    /// Логика взаимодействия для AddEditVidachi.xaml
    /// </summary>
    public partial class AddEditVidachi : Page
    {
        Vidachi vidachi;
        bool checkNew;
        public AddEditVidachi(Vidachi c, Oborudovanie preselectedEquipment = null)
        {
            InitializeComponent();

            if (c == null)
            {
                c = new Vidachi();
                checkNew = true;
                c.Date_Vidachi = DateTime.Now;
            }
            else
            {
                checkNew = false;
            }

            DataContext = vidachi = c;

            // Загрузка сотрудников
            var users = Connect.context.Users.ToList();
            combobox1.ItemsSource = users;
            combobox1.DisplayMemberPath = "Fio";
            combobox1.SelectedValuePath = "ID_Sotrudnika";

            // Загрузка оборудования (только на складе)
            var availableEquipment = Connect.context.Oborudovanie
                .Where(x => x.ID_Status == 2)
                .ToList();

            combobox2.ItemsSource = availableEquipment;
            combobox2.DisplayMemberPath = "Seriyn_Num";
            combobox2.SelectedValuePath = "ID_PC";

            // Если передано оборудование для выдачи - выбираем его
            if (preselectedEquipment != null)
            {
                combobox2.SelectedItem = preselectedEquipment;
            }

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (combobox2.SelectedItem == null)
            {
                MessageBox.Show("Выберите оборудование!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (combobox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedEquipment = combobox2.SelectedItem as Oborudovanie;
            var selectedUser = combobox1.SelectedItem as Users;
            var skladRecord = Connect.context.Sklad
                .FirstOrDefault(s => s.ID_PC == selectedEquipment.ID_PC && s.Date_Vydachi == null);

            if (skladRecord != null)
            {
                skladRecord.Date_Vydachi = DateTime.Now;
                skladRecord.Sotrudnik_Vydavshi = UserSession.CurrentUserID;
            }

            // Создаём запись о выдаче
            vidachi.ID_PC = selectedEquipment.ID_PC;
            vidachi.ID_Sotrudnika = selectedUser.ID_Sotrudnika;
            vidachi.Date_Vidachi = DPDate.SelectedDate ?? DateTime.Now;

            if (checkNew)
            {
                Connect.context.Vidachi.Add(vidachi);
            }

            // Меняем статус оборудования на "В эксплуатации"
            selectedEquipment.ID_Status = 1;

            try
            {
                Connect.context.SaveChanges();

                MessageBox.Show($"Оборудование выдано сотруднику {selectedUser.Fio}!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Nav.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.GoBack();
        }
    }
}
