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
    /// Логика взаимодействия для AddSpisanie.xaml
    /// </summary>
    public partial class AddSpisanie : Page
    {
        public AddSpisanie()
        {
            InitializeComponent();
            LoadEquipment();
        }
        private void LoadEquipment()
        {
            // Загружаем только оборудование, которое ещё не списано
            var availableEquipment = Connect.context.Oborudovanie
                .Where(x => x.Status_Oborudovania.Name_Status != "Списано")
                .ToList();

            SpisanieSerNumCB.ItemsSource = availableEquipment;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SpisanieSerNumCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите оборудование!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var equipment = SpisanieSerNumCB.SelectedItem as Oborudovanie;
            var reason = PrichinaTB.Text.Trim();

            // Проверка: указана ли причина списания
            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Укажите причину списания!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 1. Закрываем активную выдачу, если она есть
            var activeVidacha = Connect.context.Vidachi
                .FirstOrDefault(v => v.ID_PC == equipment.ID_PC && v.Date_Vozvrata == null);

            if (activeVidacha != null)
            {
                Connect.context.Vidachi.Remove(activeVidacha);
            }

            var newSpisanie = new Spisanie
            {
                ID_PC = equipment.ID_PC,
                Date_Spisania = DateTime.Now,
                Prichina_Spisania = reason,
                Sotrudnik_Spisavshi = UserSession.CurrentUserID
            };
            Connect.context.Spisanie.Add(newSpisanie);

            // 2. Обновляем оборудование (списываем)
            equipment.ID_Status = 4; // или EquipmentStatus.WrittenOff
            equipment.Prichina_Spisania = reason;
            equipment.Date_Spisania = DateTime.Now;
            equipment.Sotrudnik_Spisal = UserSession.CurrentUserID;

            try
            {
                Connect.context.SaveChanges();

                string message = "Оборудование списано!";
                if (activeVidacha != null)
                {
                    message += " Выдача автоматически закрыта.";
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
