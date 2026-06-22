using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania.Pages
{

    public partial class OborudovaniePage : Page
    {
        public OborudovaniePage()
        {
            InitializeComponent();
            LoadData();
            var statuses = Connect.context.Status_Oborudovania
        .Select(x => x.Name_Status)
        .ToList();

            // Добавляем пункт "Все статусы"
            statuses.Insert(0, "Все статусы");

            StatusCB.ItemsSource = statuses;
            StatusCB.SelectedIndex = 0; // "Все статусы" по умолчанию
        }

        private void LoadData()
        {
            var equipmentList = Connect.context.Oborudovanie
                .Include("Category_Oborudovania")
                .Include("Status_Oborudovania")
                .ToList();

            OborudovanieDG.ItemsSource = equipmentList;
        }

       
        private void HideEditButtonColumn()
        {
            // Находим колонку с кнопкой "Изменить" и скрываем её
            var editColumn = OborudovanieDG.Columns.FirstOrDefault(c => c.Header?.ToString() == "Изменить");
            if (editColumn != null)
            {
                editColumn.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new AddEditOborudovanie(null));
        }

        private void Button_Click(object sender, RoutedEventArgs e)  // Изменить
        {
            var button = sender as Button;
            var equipment = button?.Tag as Oborudovanie;

            if (equipment != null)
            {
                // Передаём оборудование на редактирование
                Nav.MainFrame.Navigate(new AddEditOborudovanie(equipment));
            }
        }

        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.GoBack();
        }


        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTB.Text;
            if (int.TryParse(searchText, out int searchNumber))
            {
                OborudovanieDG.ItemsSource = Connect.context.Oborudovanie
                    .ToList()
                    .Where(r => r.Seriyn_Num == searchNumber)
                    .ToList();
            }
            else
            {
                OborudovanieDG.ItemsSource = Connect.context.Oborudovanie.ToList();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (StatusCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус для фильтрации!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Так как ItemsSource — это список строк
            string statusName = StatusCB.SelectedItem.ToString();

            // Если выбран "Все статусы" — показываем всё оборудование
            if (statusName == "Все статусы")
            {
                LoadData();
                return;
            }

            // Фильтруем оборудование по выбранному статусу
            var filteredEquipment = Connect.context.Oborudovanie
                .Include("Category_Oborudovania")
                .Include("Status_Oborudovania")
                .Where(r => r.Status_Oborudovania.Name_Status == statusName)
                .ToList();

            OborudovanieDG.ItemsSource = filteredEquipment;
        }
    }
}
