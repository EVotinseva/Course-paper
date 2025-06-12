using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using MyFirstMauiApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MyFirstMauiApp
{
    // Модель данных для ежедневника
    public class ScheduleItem : INotifyPropertyChanged
    {
        private string _title;
        private string _description;
        private string _category;
        private DateTime _date;
        private TimeSpan? _time;
        private bool _isDone;

        public bool IsMonthlyFree { get; set; } = false;

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan? Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsDone
        {
            get => _isDone;
            set
            {
                if (_isDone != value)
                {
                    _isDone = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
            LoadItemsForDate(SelectedDate);
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
