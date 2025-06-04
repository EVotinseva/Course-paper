using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp.Views;

namespace MyFirstMauiApp.Views
{
    public partial class MonthPage : ContentPage
    {
        public MonthViewModel ViewModel;

        public MonthPage()
        {
            InitializeComponent();
            ViewModel = new MonthViewModel();
            BindingContext = ViewModel;
        }

        // ? Обработка выбора даты
        public void OnMonthStartSelected(object sender, DateChangedEventArgs e)
        {
            ViewModel.LoadItemsForMonth(e.NewDate.Date);
        }

        // ? Переход к добавлению нового дела
        public async void OnAddClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPageMonth(ViewModel));
        }

        // ? Отметка выполнения
        public void OnCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox cb && cb.BindingContext is ScheduleItem item)
            {
                ViewModel.ToggleDone(item, e.Value);
            }
        }

        // ? Удаление задачи
        public async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is ScheduleItem item)
            {
                bool confirm = await DisplayAlert("Удалить", $"Удалить дело «{item.Title}»?", "Да", "Нет");
                if (confirm)
                {
                    ViewModel.DeleteItem(item);
                }
            }
        }

        public async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is ScheduleItem item)
            {
                await Navigation.PushAsync(new AddEditPageMonth(ViewModel, item));
            }
        }

        public async void OnTaskTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is ScheduleItem item)
            {
                await Navigation.PushAsync(new TaskDetailsMonthPage(item));
            }
        }
    }
}