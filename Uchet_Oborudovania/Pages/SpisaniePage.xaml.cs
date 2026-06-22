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
    /// Логика взаимодействия для SpisaniePage.xaml
    /// </summary>
    public partial class SpisaniePage : Page
    {
        public SpisaniePage()
        {
            InitializeComponent();
            LoadData();
            LoadEmployeeFilter();
        }

        private void LoadData()
        {
            var spisanieList = Connect.context.Spisanie
        .Select(s => new
        {
            s.ID_Spisanie,
            s.Oborudovanie.Seriyn_Num,
            s.Date_Spisania,
            s.Prichina_Spisania,
            Sotrudnik_Spisavshi_FIO = Connect.context.Users
                .Where(u => u.ID_Sotrudnika == s.Sotrudnik_Spisavshi)
                .Select(u => u.Fio)
                .FirstOrDefault() ?? "Неизвестно"
        })
        .ToList();

            SpisanieDG.ItemsSource = spisanieList;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new AddSpisanie());
        }


        private void LoadEmployeeFilter()
        {
            // Загружаем всех сотрудников, которые когда-либо списывали оборудование
            var employees = Connect.context.Spisanie
                .Select(s => s.Sotrudnik_Spisavshi)
                .Distinct()
                .Join(Connect.context.Users,
                      id => id,
                      u => u.ID_Sotrudnika,
                      (id, u) => new { ID = id, Fio = u.Fio })
                .ToList();

            // Добавляем пункт "Все"
            var list = employees.Select(e => new { ID = e.ID, Fio = e.Fio }).ToList();
            list.Insert(0, new { ID = 0, Fio = "Все сотрудники" });

            cbEmployeeFilter.ItemsSource = list;
            cbEmployeeFilter.DisplayMemberPath = "Fio";
            cbEmployeeFilter.SelectedValuePath = "ID";
            cbEmployeeFilter.SelectedIndex = 0; // По умолчанию "Все"
        }

        private void FilterData(int? employeeId)
        {
            var query = Connect.context.Spisanie
                .Select(s => new
                {
                    s.ID_Spisanie,
                    s.Oborudovanie.Seriyn_Num,
                    s.Date_Spisania,
                    s.Prichina_Spisania,
                    s.Sotrudnik_Spisavshi,
                    Sotrudnik_Spisavshi_FIO = Connect.context.Users
                        .Where(u => u.ID_Sotrudnika == s.Sotrudnik_Spisavshi)
                        .Select(u => u.Fio)
                        .FirstOrDefault() ?? "Неизвестно"
                });

            // Если выбран конкретный сотрудник (не "Все")
            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(s => s.Sotrudnik_Spisavshi == employeeId.Value);
            }

            SpisanieDG.ItemsSource = query.ToList();
        }

        private void cbEmployeeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEmployeeFilter.SelectedItem == null) return;

            dynamic selected = cbEmployeeFilter.SelectedItem;
            int? employeeId = selected.ID == 0 ? null : (int?)selected.ID;

            FilterData(employeeId);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
