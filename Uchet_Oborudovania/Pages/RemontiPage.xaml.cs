using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania.Pages
{
    public partial class RemontiPage : Page
    {
        public RemontiPage()
        {
            InitializeComponent();
            LoadData();
            ApplyRoleRestrictions();
        }

        private void LoadData()
        {
            var remontiList = Connect.context.Remonti
                .Include("Oborudovanie")
                .ToList();

            RemontiDG.ItemsSource = remontiList;
            combobox1.ItemsSource = Connect.context.Remonti.Select(x => x.Status_Remonta).ToList().Distinct().ToList();
        }

        private void ApplyRoleRestrictions()
        {
            if (UserSession.IsTechnician)
            {
                AddBtn.Visibility = Visibility.Collapsed;
                DelBtn.Visibility = Visibility.Collapsed;
            }
            else if (UserSession.IsEmployee)
            {
                DelBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsTechnician)
            {
                MessageBox.Show("Техник не может создавать заявки!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Nav.MainFrame.Navigate(new AddEditRemont(null));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Изменить (для техника)
        {
            var button = sender as Button;
            var remont = button?.DataContext as Remonti;

            if (remont == null) return;

            if (UserSession.IsTechnician)
            {
                OpenStatusEditDialog(remont);
            }
            else
            {
                Nav.MainFrame.Navigate(new AddEditRemont(remont));
            }
        }

        private void OpenStatusEditDialog(Remonti remont)
        {
            var dialog = new Window
            {
                Title = "Изменение статуса ремонта",
                Width = 350,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            stackPanel.Children.Add(new TextBlock
            {
                Text = $"Заявка №{remont.ID_Remont}\nОборудование: {remont.Oborudovanie?.Model ?? "Неизвестно"}",
                Margin = new Thickness(0, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            });

            stackPanel.Children.Add(new TextBlock { Text = "Новый статус:", Margin = new Thickness(0, 0, 0, 5) });

            var statusCombo = new ComboBox { Margin = new Thickness(0, 0, 0, 15) };
            var statuses = new List<string> { "В ожидании", "Отправлен на ремонт", "Выполнен" };
            statusCombo.ItemsSource = statuses;
            statusCombo.SelectedItem = remont.Status_Remonta;
            stackPanel.Children.Add(statusCombo);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var saveBtn = new Button { Content = "Сохранить", Width = 80, Height = 30, Margin = new Thickness(0, 0, 10, 0) };
            var cancelBtn = new Button { Content = "Отмена", Width = 80, Height = 30 };

            saveBtn.Click += (s, args) =>
            {
                string newStatus = statusCombo.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(newStatus))
                {
                    MessageBox.Show("Выберите статус!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ==========================================
                // Если статус меняется на "Выполнен" — возвращаем на склад
                // ==========================================
                if (newStatus == "Выполнен")
                {
                    var equipment = remont.Oborudovanie;
                    if (equipment != null)
                    {
                        equipment.ID_Status = 2; // "На складе"
                    }
                }

                // Обновляем статус заявки
                remont.Status_Remonta = newStatus;
                remont.Date_Remonta = DateTime.Now;

                try
                {
                    Connect.context.SaveChanges();
                    LoadData();
                    dialog.Close();

                    MessageBox.Show("Статус ремонта обновлён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            cancelBtn.Click += (s, args) => dialog.Close();

            buttonPanel.Children.Add(saveBtn);
            buttonPanel.Children.Add(cancelBtn);
            stackPanel.Children.Add(buttonPanel);
            dialog.Content = stackPanel;
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsTechnician)
            {
                MessageBox.Show("Техник не может удалять заявки!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var delemployee = RemontiDG.SelectedItems.Cast<Remonti>().ToList();
            if (delemployee.Count == 0)
            {
                MessageBox.Show("Выберите запись для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить {delemployee.Count} записей?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Connect.context.Remonti.RemoveRange(delemployee);
                try
                {
                    Connect.context.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (combobox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус для фильтрации!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var filterEmployee = combobox1.SelectedItem.ToString();
            var employees = Connect.context.Remonti.ToList();
            var filteredEmployees = employees.Where(r => r.Status_Remonta == filterEmployee).ToList();
            RemontiDG.ItemsSource = filteredEmployees;
        }
    }
}