namespace MyFirstMauiApp.Views
{
    public partial class BottomNavigationBar : ContentView
    {
        public BottomNavigationBar()
        {
            InitializeComponent();
        }

        private async void OnDayClicked(object sender, EventArgs e)
        {
            string route = Shell.Current.CurrentState.Location.ToString();
            if (!route.EndsWith("day"))
            {
                await Shell.Current.GoToAsync("day");
            }
        }
        private async void OnWeekClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("week");
        }
        private async void OnMonthClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("month");
        }
    }
}