using Microsoft.Maui.Controls;
using System;

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

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            ViewModel.PreviousMonth();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            ViewModel.NextMonth();
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            if (ViewModel.CurrentFilterIndex == 0)
            {
                await Navigation.PushAsync(new AddEditPageMonth(ViewModel));
            }
            else
            {
                await Navigation.PushAsync(new AddEditPageMonth(ViewModel, null, isFreeTask: true));
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is ScheduleItem item)
            {
                await Navigation.PushAsync(new AddEditPageMonth(ViewModel, item));
            }
        }

        private void OnCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox cb && cb.BindingContext is ScheduleItem item)
            {
                ViewModel.ToggleDone(item, e.Value);
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is ScheduleItem item)
            {
                bool confirm = await DisplayAlert("Удалить", $"Удалить задачу «{item.Title}»?", "Да", "Нет");
                if (confirm)
                {
                    ViewModel.DeleteItem(item);
                }
            }
        }

        private async void OnTaskTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is ScheduleItem item)
            {
                await Navigation.PushAsync(new TaskDetailsMonthPage(item));
            }
        }

        private void OnShowAllTasksClicked(object sender, EventArgs e)
        {
            ViewModel.UpdateFilter(0);
        }

        private void OnShowFreeTasksClicked(object sender, EventArgs e)
        {
            ViewModel.UpdateFilter(1);
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}