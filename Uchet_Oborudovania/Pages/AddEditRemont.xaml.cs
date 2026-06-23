using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania.Pages
{
    public partial class AddEditRemont : Page
    {
        Remonti remonti;
        bool checkNew;

        public AddEditRemont(Remonti c)
        {
            InitializeComponent();

            if (c == null)
            {
                c = new Remonti();
                checkNew = true;
                c.Status_Remonta = "В ожидании";
                c.Date_Sozdania = DateTime.Now;
                c.ID_Sotrudnika = UserSession.CurrentUserID;
            }
            else
            {
                checkNew = false;
            }

            DataContext = remonti = c;

            // Загружаем оборудование
            var availableEquipment = Connect.context.Oborudovanie
                .Where(x => x.ID_Status != 4 & x.Status_Oborudovania.Name_Status != "Списано")
                .ToList();

            combobox2.ItemsSource = availableEquipment;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверка: выбрано ли оборудование
            if (combobox2.SelectedItem == null)
            {
                MessageBox.Show("Выберите оборудование!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(remonti.Company_Remont))
            {
                MessageBox.Show("Укажите компанию ремонта!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(remonti.Prichina_Remonta))
            {
                MessageBox.Show("Укажите причину ремонта!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedEquipment = combobox2.SelectedItem as Oborudovanie;
            remonti.ID_PC = selectedEquipment.ID_PC;

            if (checkNew)
            {
                Connect.context.Remonti.Add(remonti);
            }

            // ==========================================
            // 1. Меняем статус оборудования на "В ремонте" (ID_Status = 3)
            // ==========================================
            selectedEquipment.ID_Status = 3;

            // ==========================================
            // 2. Если оборудование было в выдаче — удаляем запись
            // ==========================================
            var activeVidacha = Connect.context.Vidachi
                .FirstOrDefault(v => v.ID_PC == selectedEquipment.ID_PC);

            if (activeVidacha != null)
            {
                Connect.context.Vidachi.Remove(activeVidacha);
            }

            try
            {
                Connect.context.SaveChanges();

                string message = "Заявка на ремонт создана!";
                if (activeVidacha != null)
                {
                    message += " Оборудование снято с выдачи.";
                }

                MessageBox.Show(message, "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Nav.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.GoBack();
        }
    }
}
