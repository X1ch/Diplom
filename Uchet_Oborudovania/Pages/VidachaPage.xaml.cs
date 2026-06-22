using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania.Pages
{
    public partial class VidachiPage : Page
    {
        public VidachiPage()
        {
            InitializeComponent();
            LoadData();

        }

        private void LoadData()
        {
            var allVidachi = Connect.context.Vidachi.ToList();
            VidachiDG.ItemsSource = allVidachi;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new AddEditVidachi(null));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vidacha = button?.Tag as Vidachi;

            var result = MessageBox.Show("Вернуть оборудование на склад?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var equipment = vidacha.Oborudovanie;

                    // 1. Удаляем запись о выдаче
                    Connect.context.Vidachi.Remove(vidacha);

                    // 2. Меняем статус оборудования на "На складе" (ID_Status = 2)
                    if (equipment != null)
                    {
                        equipment.ID_Status = 2;
                    }

                    Connect.context.SaveChanges();
                    LoadData(); // Обновляем список

                    MessageBox.Show("Оборудование возвращено на склад!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var products = Connect.context.Vidachi.AsQueryable();
            switch (SortCB.SelectedIndex)
            {
                case 1: products = products.OrderBy(p => p.Date_Vidachi); break;
                case 2: products = products.OrderByDescending(p => p.Date_Vidachi); break;
            }
            VidachiDG.ItemsSource = products.ToList();
        }
    }
}
