using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyFirstMauiApp
{
    public class WeekViewModel
    {
        public ObservableCollection<ScheduleItem> WeekItems { get; set; } = new();
        private List<ScheduleItem> AllItems { get; set; } = new();

        private const string FileName = "tasks.json";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        public DateTime StartOfWeek { get; set; } = DateTime.Today;

        public WeekViewModel()
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadAllItems();
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

        public void LoadItemsForWeek(DateTime weekStartDate)
        {
            StartOfWeek = GetStartOfWeek(weekStartDate);

            DateTime startOfWeek = StartOfWeek;
            DateTime endOfWeek = startOfWeek.AddDays(6);

            var filtered = AllItems
                .Where(i => i.Date.Date >= startOfWeek && i.Date.Date <= endOfWeek)
                .OrderBy(i => i.Date)
                .ThenBy(i => i.Time)
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                WeekItems.Clear();
                foreach (var item in filtered)
                    WeekItems.Add(item);
            });
        }

        public async void SaveItem(ScheduleItem item)
        {
            AllItems.Add(item);

            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);

            DateTime startOfWeek = StartOfWeek;
            DateTime endOfWeek = startOfWeek.AddDays(6);

            if (item.Date.Date >= startOfWeek && item.Date.Date <= endOfWeek)
            {
                MainThread.BeginInvokeOnMainThread(() => WeekItems.Add(item));
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
                WeekItems.Remove(item);
            });
        }

        public async Task SaveAll()
        {
            var json = JsonSerializer.Serialize(AllItems);
            await File.WriteAllTextAsync(FilePath, json);
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            DateTime startOfWeek = date.Date;
            while (startOfWeek.DayOfWeek != DayOfWeek.Monday)
                startOfWeek = startOfWeek.AddDays(-1);
            return startOfWeek;
        }
    }
}
