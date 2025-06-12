using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MyFirstMauiApp
{
    public class WeekViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ScheduleItem> WeekItems { get; set; } = new();
        private List<ScheduleItem> AllItems { get; set; } = new();

        private ObservableCollection<ScheduleItem> _displayedWeekItems = new();
        public ObservableCollection<ScheduleItem> DisplayedWeekItems
        {
            get => _displayedWeekItems;
            set { _displayedWeekItems = value; OnPropertyChanged(); }
        }

        private string _currentViewTitle = "Задачи на неделю";
        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set { _currentViewTitle = value; OnPropertyChanged(); }
        }

        private const string FileName = "tasks.json";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        private DateTime _startOfWeek = DateTime.Today;
        public DateTime StartOfWeek
        {
            get => _startOfWeek;
            set
            {
                var newStart = GetStartOfWeek(value);
                if (_startOfWeek != newStart)
                {
                    _startOfWeek = newStart;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WeekRangeText));
                    LoadItemsForWeek(_startOfWeek);
                }
            }
        }

        public string WeekRangeText => $"Неделя {StartOfWeek:dd.MM} - {StartOfWeek.AddDays(6):dd.MM}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public WeekViewModel() => InitializeAsync();

        private async void InitializeAsync()
        {
            await LoadAllItems();
            StartOfWeek = GetStartOfWeek(DateTime.Today);
            LoadItemsForWeek(StartOfWeek);
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
                catch { AllItems = new(); }
            }
            else AllItems = new();
        }

        public void LoadItemsForWeek(DateTime weekStartDate)
        {
            var start = GetStartOfWeek(weekStartDate);
            var end = start.AddDays(6);

            var filtered = AllItems
                .Where(i => i.Date >= start && i.Date <= end)
                .OrderBy(i => i.Date)
                .ThenBy(i => i.Time)
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                WeekItems.Clear();
                foreach (var item in filtered) WeekItems.Add(item);
                UpdateFilter(CurrentFilterIndex);
            });
        }

        public async void SaveItem(ScheduleItem item)
        {
            AllItems.Add(item);
            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);
            LoadItemsForWeek(StartOfWeek);
        }

        public async void ToggleDone(ScheduleItem item, bool isDone)
        {
            var target = AllItems.FirstOrDefault(x => x.Title == item.Title && x.Date == item.Date && x.Time == item.Time);
            if (target != null) { target.IsDone = isDone; await SaveAll(); }
        }

        public async void DeleteItem(ScheduleItem item)
        {
            AllItems.RemoveAll(x => x.Title == item.Title && x.Date == item.Date && x.Time == item.Time);
            await SaveAll();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                WeekItems.Remove(item);
                DisplayedWeekItems.Remove(item);
            });
        }

        public async Task SaveAll()
        {
            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Monday) date = date.AddDays(-1);
            return date.Date;
        }

        public void PreviousWeek() => StartOfWeek = StartOfWeek.AddDays(-7);
        public void NextWeek() => StartOfWeek = StartOfWeek.AddDays(7);

        private int _currentFilterIndex = 0;
        public int CurrentFilterIndex
        {
            get => _currentFilterIndex;
            set { _currentFilterIndex = value; OnPropertyChanged(); }
        }

        public void UpdateFilter(int selectedIndex)
        {
            CurrentFilterIndex = selectedIndex;
            if (selectedIndex == 0)
            {
                CurrentViewTitle = "Задачи на неделю";
                DisplayedWeekItems = new(WeekItems);
            }
            else if (selectedIndex == 1)
            {
                CurrentViewTitle = "Свободные задачи";
                var freeTasks = AllItems
                    .Where(item =>
                        (item.Date == DateTime.MinValue || item.Date == null)
                        && !item.IsMonthlyFree)
                    .OrderBy(item => item.Title)
                    .ToList();

                DisplayedWeekItems = new(freeTasks);
            }
        }
    }
}
