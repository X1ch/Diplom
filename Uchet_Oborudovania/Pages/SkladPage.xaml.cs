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
    /// Логика взаимодействия для SkladPage.xaml
    /// </summary>
    public partial class SkladPage : Page
    {
        public SkladPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var skladEquipment = Connect.context.Sklad
            .Select(x => x.Oborudovanie)        // Берём оборудование
            .Where(o => o.ID_Status == 2)       // Статус "На складе"
            .ToList();

            SkladDG.ItemsSource = skladEquipment;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var equipment = button?.Tag as Oborudovanie;

            if (equipment != null)
            {
                // Передаём оборудование на выдачу
                Nav.MainFrame.Navigate(new AddEditVidachi(null, equipment));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
