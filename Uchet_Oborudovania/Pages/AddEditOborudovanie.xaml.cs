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
    /// Логика взаимодействия для AddEditOborudovanie.xaml
    /// </summary>
    public partial class AddEditOborudovanie : Page
    {
        Oborudovanie _currentEquipment;
        bool _isEditMode;

        public AddEditOborudovanie(Oborudovanie equipment)
        {
            InitializeComponent();
            LoadCategories();

            if (equipment == null)
            {
                // Режим добавления
                _isEditMode = false;
                TitleText.Text = "➕ Добавление оборудования";
                _currentEquipment = new Oborudovanie();
            }
            else
            {
                // Режим редактирования
                _isEditMode = true;
                TitleText.Text = "✏️ Редактирование оборудования";
                _currentEquipment = equipment;

                // Заполняем поля
                txtSeriynNum.Text = equipment.Seriyn_Num.ToString();
                txtModel.Text = equipment.Model;
                dpDateBuy.SelectedDate = equipment.Date_Buy;
                txtGarantia.Text = equipment.Garantia;

                // Выбираем категорию
                if (equipment.Category_Oborudovania != null)
                {
                    cbCategory.SelectedItem = equipment.Category_Oborudovania;
                }
            }
        }

        private void LoadCategories()
        {
            cbCategory.ItemsSource = Connect.context.Category_Oborudovania.ToList();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей
            if (string.IsNullOrWhiteSpace(txtSeriynNum.Text))
            {
                MessageBox.Show("Введите серийный номер!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Введите модель!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Заполняем данные
            _currentEquipment.Seriyn_Num = int.Parse(txtSeriynNum.Text);
            _currentEquipment.Category_Oborudovania = cbCategory.SelectedItem as Category_Oborudovania;
            _currentEquipment.Model = txtModel.Text;
            _currentEquipment.Date_Buy = dpDateBuy.SelectedDate ?? DateTime.Now;
            _currentEquipment.Garantia = txtGarantia.Text;

            if (!_isEditMode)
            {
                // Добавляем оборудование
                _currentEquipment.ID_Status = 2; // Статус "На складе"
                Connect.context.Oborudovanie.Add(_currentEquipment);
                Connect.context.SaveChanges(); // Сохраняем, чтобы получить ID_PC

                // ==========================================
                // Добавляем запись на склад
                // ==========================================
                var skladRecord = new Sklad
                {
                    ID_PC = _currentEquipment.ID_PC,
                    Date_Postuplenia = DateTime.Now,
                    Prichina = "Новое оборудование",
                    Sotrudnik_Postavivshi = UserSession.CurrentUserID
                };
                Connect.context.Sklad.Add(skladRecord);
            }
            try
            {
                Connect.context.SaveChanges();

                string message = _isEditMode
                    ? "Данные оборудования обновлены!"
                    : "Оборудование успешно добавлено на склад!";

                MessageBox.Show(message, "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Nav.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.GoBack();
        }
    }
}



