using Microsoft.Maui.Controls;

namespace MyFirstMauiApp.Views
{
    public partial class BottomNavigationBar : ContentView
    {
        public BottomNavigationBar()
        {
            InitializeComponent();
        }

        private INavigation GetNavigation()
        {
            // Берём текущую Navigation из текущей страницы
            if (Application.Current.MainPage is NavigationPage navPage)
            {
                return navPage.Navigation;
            }
            return null;
        }

        private async void OnDayClicked(object sender, EventArgs e)
        {
            var navigation = GetNavigation();
            if (navigation != null)
            {
                await navigation.PushAsync(new SchedulePage());
            }
        }

        private async void OnWeekClicked(object sender, EventArgs e)
        {
            var navigation = GetNavigation();
            if (navigation != null)
            {
                await navigation.PushAsync(new ContentPage
                {
                    Title = "Неделя",
                    Content = new Label
                    {
                        Text = "Страница Неделя",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 24,
                        TextColor = Colors.Black
                    }
                });
            }
        }

        private async void OnMonthClicked(object sender, EventArgs e)
        {
            var navigation = GetNavigation();
            if (navigation != null)
            {
                await navigation.PushAsync(new ContentPage
                {
                    Title = "Месяц",
                    Content = new Label
                    {
                        Text = "Страница Месяц",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 24,
                        TextColor = Colors.Black
                    }
                });
            }
        }
    }
}
