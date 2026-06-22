using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using Uchet_Oborudovania.AppData;

namespace Uchet_Oborudovania.Pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        // ==========================================
        // ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ОФОРМЛЕНИЯ ОТЧЕТА
        // ==========================================
        private void FormatReport(Excel.Worksheet worksheet, string title, int lastRow, int lastColumn)
        {
            // 1. Заголовок отчета
            worksheet.Cells[1, 1] = title;
            Excel.Range titleRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, lastColumn]];
            titleRange.Merge();
            titleRange.Font.Bold = true;
            titleRange.Font.Size = 18;
            titleRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            titleRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            // 2. Пустая строка между заголовком и таблицей (уже есть)

            // 3. Дата формирования отчета
            int dateRow = lastRow + 2;
            worksheet.Cells[dateRow, 1] = $"Дата формирования отчета: {DateTime.Now:dd.MM.yyyy HH:mm}";
            worksheet.Cells[dateRow, 1].Font.Bold = true;
            worksheet.Cells[dateRow, 1].Font.Size = 12;
            worksheet.Range[worksheet.Cells[dateRow, 1], worksheet.Cells[dateRow, lastColumn]].Merge();

            // 4. Место для подписи
            int signRow = dateRow + 2;
            worksheet.Cells[signRow, 1] = "Подпись: __________________";
            worksheet.Cells[signRow, 1].Font.Size = 12;
            worksheet.Range[worksheet.Cells[signRow, 1], worksheet.Cells[signRow, lastColumn]].Merge();

            // 5. Место для печати
            int printRow = signRow + 1;
            worksheet.Cells[printRow, 1] = "М.П.";
            worksheet.Cells[printRow, 1].Font.Size = 12;
            worksheet.Cells[printRow, 1].Font.Bold = true;
            worksheet.Range[worksheet.Cells[printRow, 1], worksheet.Cells[printRow, lastColumn]].Merge();

            // ==========================================
            // НАСТРОЙКА ПЕЧАТИ
            // ==========================================

            // 6. Устанавливаем границы печати (вся область с данными)
            Excel.Range printArea = worksheet.Range[
                worksheet.Cells[1, 1],
                worksheet.Cells[printRow + 1, lastColumn]
            ];
            worksheet.PageSetup.PrintArea = printArea.Address;

            // 7. Ориентация страницы: книжная или альбомная
            // Если колонок мало (< 6) — книжная, иначе альбомная
            if (lastColumn <= 6)
            {
                worksheet.PageSetup.Orientation = Excel.XlPageOrientation.xlPortrait; // Книжная
            }
            else
            {
                worksheet.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape; // Альбомная
            }

            // 8. Всё помещается на одну страницу
            worksheet.PageSetup.FitToPagesWide = 1;   // По ширине — 1 страница
            worksheet.PageSetup.FitToPagesTall = 1;   // По высоте — 1 страница

            // 9. Поля страницы (сверху, снизу, слева, справа)
            worksheet.PageSetup.TopMargin = 20;        // Верхнее поле (в пунктах)
            worksheet.PageSetup.BottomMargin = 20;     // Нижнее поле
            worksheet.PageSetup.LeftMargin = 15;       // Левое поле
            worksheet.PageSetup.RightMargin = 15;      // Правое поле

            // 10. Центрирование на странице
            worksheet.PageSetup.CenterHorizontally = true;
            worksheet.PageSetup.CenterVertically = false;

            // 11. Авто-подбор ширины колонок
            worksheet.Columns.AutoFit();

            // 12. Выделяем границы таблицы
            Excel.Range tableRange = worksheet.Range[
                worksheet.Cells[2, 1],
                worksheet.Cells[lastRow, lastColumn]
            ];
            tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            tableRange.Font.Size = 11;

            // 13. Выравниваем заголовки таблицы по центру
            Excel.Range headerRange = worksheet.Range[
                worksheet.Cells[2, 1],
                worksheet.Cells[2, lastColumn]
            ];
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.Interior.Color = System.Drawing.Color.LightGray;
            headerRange.Font.Bold = true;
            headerRange.Font.Size = 12;
        }
        // ==========================================
        // 1. Отчет о выданном оборудовании
        // ==========================================
        private void RepoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportData = (from v in Connect.context.Vidachi
                                  join s in Connect.context.Users on v.ID_Sotrudnika equals s.ID_Sotrudnika
                                  join o in Connect.context.Oborudovanie on v.ID_PC equals o.ID_PC
                                  select new
                                  {
                                      v.ID_Vidachi,
                                      Сотрудник = s.Fio,
                                      СерийныйНомер = o.Seriyn_Num,
                                      ДатаВыдачи = v.Date_Vidachi
                                  })
                                  .OrderBy(x => x.ДатаВыдачи)
                                  .ToList();

                if (!reportData.Any())
                {
                    MessageBox.Show("Нет данных для отчета о выданном оборудовании", "Информация",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Отчет_о_выданном_оборудовании_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    excelApp.DisplayAlerts = false;

                    try
                    {
                        Excel.Workbook workbook = excelApp.Workbooks.Add();
                        Excel.Worksheet worksheet = workbook.Sheets[1];
                        worksheet.Name = "Выданное оборудование";

                        worksheet.Cells[2, 1] = "№";
                        worksheet.Cells[2, 2] = "Сотрудник";
                        worksheet.Cells[2, 3] = "Серийный номер";
                        worksheet.Cells[2, 4] = "Дата выдачи";

                        Excel.Range headerRange = worksheet.Range["A2:D2"];
                        headerRange.Font.Bold = true;
                        headerRange.Font.Size = 12;
                        headerRange.Interior.Color = System.Drawing.Color.LightGray;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        int row = 3;
                        foreach (var item in reportData)
                        {
                            worksheet.Cells[row, 1] = item.ID_Vidachi;
                            worksheet.Cells[row, 2] = item.Сотрудник;
                            worksheet.Cells[row, 3] = item.СерийныйНомер;
                            worksheet.Cells[row, 4] = item.ДатаВыдачи.ToString("dd.MM.yyyy HH:mm");
                            row++;
                        }

                        Excel.Range dataRange = worksheet.Range[$"A2:D{row - 1}"];
                        dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        int lastRow = row - 1;
                        FormatReport(worksheet, "ОТЧЕТ ПО ВЫДАННОМУ ОБОРУДОВАНИЮ", lastRow, 4);

                        workbook.SaveAs(saveFileDialog.FileName);
                        workbook.Close();

                        MessageBox.Show($"Отчет успешно сохранен!\n{saveFileDialog.FileName}",
                                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==========================================
        // 2. Отчет о ремонте оборудования
        // ==========================================
        private void RepRemBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportData = (from r in Connect.context.Remonti
                                  join o in Connect.context.Oborudovanie on r.ID_PC equals o.ID_PC
                                  select new
                                  {
                                      r.ID_Remont,
                                      СерийныйНомер = o.Seriyn_Num,
                                      r.Company_Remont,
                                      r.Prichina_Remonta,
                                      r.Date_Remonta,
                                      r.Status_Remonta
                                  })
                                  .OrderBy(x => x.Date_Remonta)
                                  .ToList();

                if (!reportData.Any())
                {
                    MessageBox.Show("Нет данных для отчета о ремонте оборудования", "Информация",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Отчет_о_ремонте_оборудования_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    excelApp.DisplayAlerts = false;

                    try
                    {
                        Excel.Workbook workbook = excelApp.Workbooks.Add();
                        Excel.Worksheet worksheet = workbook.Sheets[1];
                        worksheet.Name = "Ремонт оборудования";

                        worksheet.Cells[2, 1] = "№";
                        worksheet.Cells[2, 2] = "Серийный номер";
                        worksheet.Cells[2, 3] = "Компания ремонта";
                        worksheet.Cells[2, 4] = "Причина ремонта";
                        worksheet.Cells[2, 5] = "Дата ремонта";
                        worksheet.Cells[2, 6] = "Статус";

                        Excel.Range headerRange = worksheet.Range["A2:F2"];
                        headerRange.Font.Bold = true;
                        headerRange.Font.Size = 12;
                        headerRange.Interior.Color = System.Drawing.Color.LightGray;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        int row = 3;
                        foreach (var item in reportData)
                        {
                            worksheet.Cells[row, 1] = item.ID_Remont;
                            worksheet.Cells[row, 2] = item.СерийныйНомер;
                            worksheet.Cells[row, 3] = item.Company_Remont;
                            worksheet.Cells[row, 4] = item.Prichina_Remonta;
                            worksheet.Cells[row, 5] = item.Date_Remonta.ToString("dd.MM.yyyy") ?? "";
                            worksheet.Cells[row, 6] = item.Status_Remonta ?? "Не указан";
                            row++;
                        }

                        Excel.Range dataRange = worksheet.Range[$"A2:F{row - 1}"];
                        dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        int lastRow = row - 1;
                        FormatReport(worksheet, "ОТЧЕТ ПО РЕМОНТУ ОБОРУДОВАНИЯ", lastRow, 6);

                        workbook.SaveAs(saveFileDialog.FileName);
                        workbook.Close();

                        MessageBox.Show($"Отчет успешно сохранен!\n{saveFileDialog.FileName}",
                                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==========================================
        // 3. Отчет о списанном оборудовании
        // ==========================================
        private void RepSpisBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportData = (from s in Connect.context.Spisanie
                                  join o in Connect.context.Oborudovanie on s.ID_PC equals o.ID_PC
                                  join u in Connect.context.Users on s.Sotrudnik_Spisavshi equals u.ID_Sotrudnika
                                  select new
                                  {
                                      s.ID_Spisanie,
                                      СерийныйНомер = o.Seriyn_Num,
                                      s.Date_Spisania,
                                      s.Prichina_Spisania,
                                      Сотрудник = u.Fio
                                  })
                                  .OrderBy(x => x.Date_Spisania)
                                  .ToList();

                if (!reportData.Any())
                {
                    MessageBox.Show("Нет данных для отчета о списанном оборудовании", "Информация",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Отчет_о_списанном_оборудовании_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    excelApp.DisplayAlerts = false;

                    try
                    {
                        Excel.Workbook workbook = excelApp.Workbooks.Add();
                        Excel.Worksheet worksheet = workbook.Sheets[1];
                        worksheet.Name = "Списанное оборудование";

                        worksheet.Cells[2, 1] = "№";
                        worksheet.Cells[2, 2] = "Серийный номер";
                        worksheet.Cells[2, 3] = "Дата списания";
                        worksheet.Cells[2, 4] = "Причина списания";
                        worksheet.Cells[2, 5] = "Сотрудник";

                        Excel.Range headerRange = worksheet.Range["A2:E2"];
                        headerRange.Font.Bold = true;
                        headerRange.Font.Size = 12;
                        headerRange.Interior.Color = System.Drawing.Color.LightGray;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        int row = 3;
                        foreach (var item in reportData)
                        {
                            worksheet.Cells[row, 1] = item.ID_Spisanie;
                            worksheet.Cells[row, 2] = item.СерийныйНомер;
                            worksheet.Cells[row, 3] = item.Date_Spisania.ToString("dd.MM.yyyy") ?? "";
                            worksheet.Cells[row, 4] = item.Prichina_Spisania;
                            worksheet.Cells[row, 5] = item.Сотрудник;
                            row++;
                        }

                        Excel.Range dataRange = worksheet.Range[$"A2:E{row - 1}"];
                        dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        int lastRow = row - 1;
                        FormatReport(worksheet, "ОТЧЕТ ПО СПИСАННОМУ ОБОРУДОВАНИЮ", lastRow, 5);

                        workbook.SaveAs(saveFileDialog.FileName);
                        workbook.Close();

                        MessageBox.Show($"Отчет успешно сохранен!\n{saveFileDialog.FileName}",
                                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==========================================
        // 4. Отчет по оборудованию
        // ==========================================
        private void RepOborudBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportData = Connect.context.Oborudovanie
                    .Include("Category_Oborudovania")
                    .Include("Status_Oborudovania")
                    .Select(o => new
                    {
                        o.ID_PC,
                        o.Seriyn_Num,
                        Категория = o.Category_Oborudovania.NameCategory,
                        o.Model,
                        o.Date_Buy,
                        o.Garantia,
                        Статус = o.Status_Oborudovania.Name_Status
                    })
                    .OrderBy(x => x.ID_PC)
                    .ToList();

                if (!reportData.Any())
                {
                    MessageBox.Show("Нет данных для отчета по оборудованию", "Информация",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Отчет_по_оборудованию_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    excelApp.DisplayAlerts = false;

                    try
                    {
                        Excel.Workbook workbook = excelApp.Workbooks.Add();
                        Excel.Worksheet worksheet = workbook.Sheets[1];
                        worksheet.Name = "Оборудование";

                        worksheet.Cells[2, 1] = "№";
                        worksheet.Cells[2, 2] = "Серийный номер";
                        worksheet.Cells[2, 3] = "Категория";
                        worksheet.Cells[2, 4] = "Модель";
                        worksheet.Cells[2, 5] = "Дата покупки";
                        worksheet.Cells[2, 6] = "Гарантия";
                        worksheet.Cells[2, 7] = "Статус";

                        Excel.Range headerRange = worksheet.Range["A2:G2"];
                        headerRange.Font.Bold = true;
                        headerRange.Font.Size = 12;
                        headerRange.Interior.Color = System.Drawing.Color.LightGray;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        int row = 3;
                        foreach (var item in reportData)
                        {
                            worksheet.Cells[row, 1] = item.ID_PC;
                            worksheet.Cells[row, 2] = item.Seriyn_Num;
                            worksheet.Cells[row, 3] = item.Категория;
                            worksheet.Cells[row, 4] = item.Model;
                            worksheet.Cells[row, 5] = item.Date_Buy.ToString("dd.MM.yyyy") ?? "";
                            worksheet.Cells[row, 6] = item.Garantia;
                            worksheet.Cells[row, 7] = item.Статус;
                            row++;
                        }

                        Excel.Range dataRange = worksheet.Range[$"A2:G{row - 1}"];
                        dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        int lastRow = row - 1;
                        FormatReport(worksheet, "ОТЧЕТ ПО ОБОРУДОВАНИЮ", lastRow, 7);

                        workbook.SaveAs(saveFileDialog.FileName);
                        workbook.Close();

                        MessageBox.Show($"Отчет успешно сохранен!\n{saveFileDialog.FileName}",
                                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}