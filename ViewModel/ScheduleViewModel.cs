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
        private List<ScheduleItem> AllItems { get; set; } = new();

        private const string FileName = "tasks.json";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        public DateTime SelectedDate { get; set; } = DateTime.Today;

        public ScheduleViewModel()
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadAllItems();
            LoadItemsForDate(SelectedDate);
        }

        public async Task LoadAllItems()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(FilePath);
                    AllItems = JsonSerializer.Deserialize<List<ScheduleItem>>(json) ?? new();
                }
                catch
                {
                    AllItems = new List<ScheduleItem>();
                }
            }
            else
            {
                AllItems = new List<ScheduleItem>();
            }
        }

        public void LoadItemsForDate(DateTime date)
        {
            SelectedDate = date;

            var filtered = AllItems
                .Where(i => i.Date.Date == date.Date)
                .OrderBy(i => i.Time)
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TodayItems.Clear();
                foreach (var item in filtered)
                    TodayItems.Add(item);
            });
        }

        public async void SaveItem(ScheduleItem item)
        {
            AllItems.Add(item);

            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);

            // Обновим список, если задача на выбранную дату
            if (item.Date.Date == SelectedDate.Date)
            {
                MainThread.BeginInvokeOnMainThread(() => TodayItems.Add(item));
            }
        }

        public async void ToggleDone(ScheduleItem item, bool isDone)
        {
            // Найдём оригинальный элемент
            var target = AllItems.FirstOrDefault(x =>
                x.Title == item.Title &&
                x.Date == item.Date &&
                x.Time == item.Time);

            if (target != null)
            {
                target.IsDone = isDone;
                await SaveAll();
            }
        }

        public async void DeleteItem(ScheduleItem item)
        {
            // Убираем из AllItems
            AllItems.RemoveAll(x =>
                x.Title == item.Title &&
                x.Date == item.Date &&
                x.Time == item.Time);

            // Сохраняем в файл
            await SaveAll();

            // Убираем из TodayItems (если на текущем экране)
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TodayItems.Remove(item);
            });
        }

        public async Task SaveAll()
        {
            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}
