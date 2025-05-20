using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp.Views;       

namespace MyFirstMauiApp.Views
{
    public partial class SchedulePage : ContentPage
    {
        private ScheduleViewModel ViewModel;

        public SchedulePage()
        {
            InitializeComponent();
            ViewModel = new ScheduleViewModel();
            BindingContext = ViewModel;
        }

        // ? Обработка выбора даты
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            ViewModel.LoadItemsForDate(e.NewDate.Date);
        }

        // ? Переход к добавлению нового дела
        private async void OnAddClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPage(ViewModel));
        }

        // ? Отметка выполнения
        private void OnCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox cb && cb.BindingContext is ScheduleItem item)
            {
                ViewModel.ToggleDone(item, e.Value);
            }
        }

        // ? Удаление задачи
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is ScheduleItem item)
            {
                bool confirm = await DisplayAlert("Удалить", $"Удалить дело «{item.Title}»?", "Да", "Нет");
                if (confirm)
                {
                    ViewModel.DeleteItem(item);
                }
            }
        }
    }
}