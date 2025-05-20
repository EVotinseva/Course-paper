using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using MyFirstMauiApp.Views;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;

namespace MyFirstMauiApp
{
    // Модель данных для ежедневника
    public class ScheduleItem
    {
        public string Title { get; set; } // Название дела
        public string Description { get; set; } // Подробности
        public string Category { get; set; } // Категория: личное, работа и т.д.
        public DateTime Date { get; set; } // Дата
        public TimeSpan Time { get; set; } // Время
        public bool IsDone { get; set; } // Выполнено или нет
    }

    public class ScheduleViewModel
    {
        public ObservableCollection<ScheduleItem> TodayItems { get; set; } = new();
        private const string FileName = "tasks.json";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        public ScheduleViewModel()
        {
            LoadItems();
        }

        public async void LoadItems()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(FilePath);
                    var allItems = JsonSerializer.Deserialize<List<ScheduleItem>>(json) ?? new();
                    var today = DateTime.Today;

                    var todayList = allItems
                        .Where(i => i.Date.Date == today)
                        .OrderBy(i => i.Time)
                        .ToList();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TodayItems.Clear();
                        foreach (var item in todayList)
                            TodayItems.Add(item);
                    });
                }
                catch
                {
                    // Ошибка чтения — игнорируем
                }
            }
        }
        public async void LoadItemsForDate(DateTime date)
        {
            if (File.Exists(FilePath))
            {
                var json = await File.ReadAllTextAsync(FilePath);
                var allItems = JsonSerializer.Deserialize<List<ScheduleItem>>(json) ?? new();

                var filtered = allItems
                    .Where(i => i.Date.Date == date)
                    .OrderBy(i => i.Time)
                    .ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TodayItems.Clear();
                    foreach (var item in filtered)
                        TodayItems.Add(item);
                });
            }
        }
        public async void SaveItem(ScheduleItem item)
        {
            List<ScheduleItem> allItems = new();

            // Читаем старые
            if (File.Exists(FilePath))
            {
                var jsonOld = await File.ReadAllTextAsync(FilePath);
                allItems = JsonSerializer.Deserialize<List<ScheduleItem>>(jsonOld) ?? new();
            }

            allItems.Add(item);

            var jsonNew = JsonSerializer.Serialize(allItems);
            await File.WriteAllTextAsync(FilePath, jsonNew);

            // Обновим список, если задача на сегодня
            if (item.Date.Date == DateTime.Today)
            {
                MainThread.BeginInvokeOnMainThread(() => TodayItems.Add(item));
            }
        }
        public void ToggleDone(ScheduleItem item, bool isDone)
        {
            item.IsDone = isDone;
            SaveAll(); // сохраняем весь список
        }

        public void DeleteItem(ScheduleItem item)
        {
            TodayItems.Remove(item);

            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                var allItems = JsonSerializer.Deserialize<List<ScheduleItem>>(json) ?? new();
                allItems.RemoveAll(x => x.Title == item.Title && x.Date == item.Date && x.Time == item.Time);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(allItems));
            }
        }
        public void SaveAll()
        {
            var json = JsonSerializer.Serialize(TodayItems.ToList());
            File.WriteAllText(FilePath, json);
        }

    }
}
