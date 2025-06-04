using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyFirstMauiApp
{
    public class MonthViewModel
    {
        public ObservableCollection<ScheduleItem> MonthItems { get; set; } = new();
        private List<ScheduleItem> AllItems { get; set; } = new();

        private const string FileName = "tasks.json";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        public DateTime SelectedDate { get; set; } = DateTime.Today;

        public MonthViewModel()
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadAllItems();
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

        public void LoadItemsForMonth(DateTime monthDate)
        {
            SelectedDate = monthDate;

            DateTime startOfMonth = GetStartOfMonth(monthDate);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var filtered = AllItems
                .Where(i => i.Date.Date >= startOfMonth && i.Date.Date <= endOfMonth)
                .OrderBy(i => i.Date)
                .ThenBy(i => i.Time)
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                MonthItems.Clear();
                foreach (var item in filtered)
                    MonthItems.Add(item);
            });
        }

        public async void SaveItem(ScheduleItem item)
        {
            AllItems.Add(item);

            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);

            DateTime startOfMonth = GetStartOfMonth(SelectedDate);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            if (item.Date.Date >= startOfMonth && item.Date.Date <= endOfMonth)
            {
                MainThread.BeginInvokeOnMainThread(() => MonthItems.Add(item));
            }
        }

        public async void ToggleDone(ScheduleItem item, bool isDone)
        {
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
            AllItems.RemoveAll(x =>
                x.Title == item.Title &&
                x.Date == item.Date &&
                x.Time == item.Time);

            await SaveAll();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                MonthItems.Remove(item);
            });
        }

        public async Task SaveAll()
        {
            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);
        }

        private DateTime GetStartOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
    }
}
