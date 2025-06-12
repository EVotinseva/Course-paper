using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using MyFirstMauiApp.Views;
using Plugin.LocalNotification;
using System.Globalization;

namespace MyFirstMauiApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Запрашиваем разрешение на уведомления
            _ = RequestNotificationPermissionAsync();

            // Можно подписаться на событие "по клику по уведомлению" (необязательно)
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new SchedulePage()));
        }

        /// <summary>
        /// Запросить разрешение на уведомления (Android 13+ и iOS)
        /// </summary>
        private async Task RequestNotificationPermissionAsync()
        {
            var granted = await LocalNotificationCenter.Current.RequestNotificationPermission();

            if (granted)
            {
                System.Diagnostics.Debug.WriteLine("✅ Разрешение на уведомления предоставлено.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Разрешение на уведомления ОТКАЗАНО.");
            }
        }

        /// <summary>
        /// Событие при клике по уведомлению
        /// </summary>
        private void OnNotificationTapped(Plugin.LocalNotification.EventArgs.NotificationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"👉 Пользователь нажал на уведомление: {e.Request.Title}");
            // Можно здесь сделать переход на нужную страницу
        }
    }

    /// <summary>
    /// Конвертер для зачеркивания выполненных задач
    /// </summary>
    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? TextDecorations.Strikethrough : TextDecorations.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FilterToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int currentFilter = (int)value;
            int targetFilter = int.Parse(parameter.ToString());

            return currentFilter == targetFilter ? Color.FromArgb("#3E0FB6") : Color.FromArgb("#9E9E9E");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date != DateTime.MinValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}