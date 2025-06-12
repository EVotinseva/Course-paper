using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MyFirstMauiApp
{
    public class MonthViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ScheduleItem> MonthItems { get; set; } = new();
        private List<ScheduleItem> AllItems { get; set; } = new();

        private ObservableCollection<ScheduleItem> _displayedMonthItems = new();
        public ObservableCollection<ScheduleItem> DisplayedMonthItems
        {
            get => _displayedMonthItems;
            set { _displayedMonthItems = value; OnPropertyChanged(); }
        }

        private string _currentViewTitle = "Задачи на месяц";
        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set { _currentViewTitle = value; OnPropertyChanged(); }
        }

        private string _monthRangeText;
        public string MonthRangeText
        {
            get => _monthRangeText;
            set { _monthRangeText = value; OnPropertyChanged(); }
        }

        private const string FileName = "tasks.json";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                    UpdateMonthRangeText();
                    LoadItemsForMonth(_selectedDate);
                }
            }
        }

        private int _currentFilterIndex = 0;
        public int CurrentFilterIndex
        {
            get => _currentFilterIndex;
            set { _currentFilterIndex = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public MonthViewModel() => InitializeAsync();

        private async void InitializeAsync()
        {
            await LoadAllItems();
            SelectedDate = DateTime.Today;
            UpdateMonthRangeText();
            LoadItemsForMonth(SelectedDate);
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

        public void LoadItemsForMonth(DateTime monthDate)
        {
            DateTime start = GetStartOfMonth(monthDate);
            DateTime end = start.AddMonths(1).AddDays(-1);

            var filtered = AllItems
                .Where(i => i.Date.Date >= start && i.Date.Date <= end)
                .OrderBy(i => i.Date)
                .ThenBy(i => i.Time)
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                MonthItems.Clear();
                foreach (var item in filtered) MonthItems.Add(item);
                UpdateFilter(CurrentFilterIndex);
            });
        }

        public async void SaveItem(ScheduleItem item)
        {
            AllItems.Add(item);
            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);
            LoadItemsForMonth(SelectedDate);
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
                MonthItems.Remove(item);
                DisplayedMonthItems.Remove(item);
            });
        }

        public async Task SaveAll()
        {
            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);
        }

        private DateTime GetStartOfMonth(DateTime date) =>
            new(date.Year, date.Month, 1);

        public void PreviousMonth() => SelectedDate = SelectedDate.AddMonths(-1);
        public void NextMonth() => SelectedDate = SelectedDate.AddMonths(1);

        private void UpdateMonthRangeText()
        {
            var name = SelectedDate.ToString("MMMM yyyy");
            if (!string.IsNullOrEmpty(name))
                name = char.ToUpper(name[0]) + name.Substring(1);
            MonthRangeText = name;
        }

        public void UpdateFilter(int selectedIndex)
        {
            CurrentFilterIndex = selectedIndex;

            if (selectedIndex == 0)
            {
                CurrentViewTitle = "Задачи на месяц";
                DisplayedMonthItems = new(MonthItems);
            }
            else if (selectedIndex == 1)
            {
                CurrentViewTitle = "Свободные задачи";
                var freeTasks = AllItems
                    .Where(item =>
                        (item.Date == DateTime.MinValue || item.Date == null)
                        && item.IsMonthlyFree)
                    .OrderBy(item => item.Title)
                    .ToList();

                DisplayedMonthItems = new(freeTasks);
            }
        }
    }
}