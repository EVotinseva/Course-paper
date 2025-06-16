using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp.Views;

namespace MyFirstMauiApp.Views
{
    public partial class WeekPage : ContentPage
    {
        public WeekViewModel ViewModel;

        public WeekPage()
        {
            InitializeComponent();
            ViewModel = new WeekViewModel();
            BindingContext = ViewModel;
        }

        public void OnPreviousWeekClicked(object sender, EventArgs e)
        {
            ViewModel.PreviousWeek();
        }

        public void OnNextWeekClicked(object sender, EventArgs e)
        {
            ViewModel.NextWeek();
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            if (BindingContext is WeekViewModel vm)
            {
                if (vm.CurrentFilterIndex == 0)
                {
                    // Добавление задачи на неделю (с датой)
                    await Navigation.PushAsync(new AddEditPageWeek(vm));
                }
                else if (vm.CurrentFilterIndex == 1)
                {
                    // Добавление "Свободной задачи" (без даты)
                    await Navigation.PushAsync(new AddEditPageWeek(vm, null, isFreeTask: true));
                }
            }
        }

        public void OnCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox cb && cb.BindingContext is ScheduleItem item)
            {
                ViewModel.ToggleDone(item, e.Value);
            }
        }

        public async void OnDeleteClicked(object sender, EventArgs e)
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

        public async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is ScheduleItem item)
            {
                await Navigation.PushAsync(new AddEditPageWeek(ViewModel, item));
            }
        }

        public async void OnTaskTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is ScheduleItem item)
            {
                await Navigation.PushAsync(new TaskDetailsWeekPage(item));
            }
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker != null && picker.SelectedIndex >= 0)
            {
                ViewModel.UpdateFilter(picker.SelectedIndex);
            }
        }

        private void OnShowAllTasksClicked(object sender, EventArgs e)
        {
            if (BindingContext is WeekViewModel vm)
            {
                vm.UpdateFilter(0);
            }
        }

        private void OnShowFreeTasksClicked(object sender, EventArgs e)
        {
            if (BindingContext is WeekViewModel vm)
            {
                vm.UpdateFilter(1);
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}