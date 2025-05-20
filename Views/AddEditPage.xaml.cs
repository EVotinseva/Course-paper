using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp;
using MyFirstMauiApp.Views;

namespace MyFirstMauiApp.Views
{
    public partial class AddEditPage : ContentPage
    {
        private ScheduleViewModel _viewModel;

        public AddEditPage(ScheduleViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var newItem = new ScheduleItem
            {
                Title = titleEntry.Text,
                Description = descriptionEditor.Text,
                Category = categoryPicker.SelectedItem?.ToString() ?? "Без категории",
                Date = datePicker.Date,
                Time = timePicker.Time,
                IsDone = false
            };

            // Добавляем в список, если задача на сегодня
            if (newItem.Date.Date == DateTime.Today)
            {
                _viewModel.SaveItem(newItem);
            }

            await DisplayAlert("Добавлено", $"«{newItem.Title}» добавлено в список", "ОК");
            await Navigation.PopAsync();
        }
    }
}