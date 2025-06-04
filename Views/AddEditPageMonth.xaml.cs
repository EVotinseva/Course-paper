using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp;
using MyFirstMauiApp.Views;

namespace MyFirstMauiApp.Views
{
    public partial class AddEditPageMonth : ContentPage
    {
        private MonthViewModel _viewModel;
        private ScheduleItem _editingItem;

        public AddEditPageMonth(MonthViewModel viewModel, ScheduleItem editingItem = null)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _editingItem = editingItem;

            if (_editingItem != null)
            {
                Title = "Редактирование дела";
                titleEntry.Text = _editingItem.Title;
                descriptionEditor.Text = _editingItem.Description;
                categoryPicker.SelectedItem = _editingItem.Category;
                datePicker.Date = _editingItem.Date;
                timePicker.Time = _editingItem.Time;
            }
            else
            {
                Title = "Новое дело";
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_editingItem != null)
            {
                // Режим редактирования
                _editingItem.Title = titleEntry.Text;
                _editingItem.Description = descriptionEditor.Text;
                _editingItem.Category = categoryPicker.SelectedItem?.ToString() ?? "Без категории";
                _editingItem.Date = datePicker.Date;
                _editingItem.Time = timePicker.Time;

                await _viewModel.SaveAll(); // сохраняем все изменения
                _viewModel.LoadItemsForMonth(_viewModel.SelectedDate);

                await DisplayAlert("Изменено", $"«{_editingItem.Title}» обновлено", "ОК");
            }
            else
            {
                // Режим создания новой задачи
                var newItem = new ScheduleItem
                {
                    Title = titleEntry.Text,
                    Description = descriptionEditor.Text,
                    Category = categoryPicker.SelectedItem?.ToString() ?? "Без категории",
                    Date = datePicker.Date,
                    Time = timePicker.Time,
                    IsDone = false
                };

                _viewModel.SaveItem(newItem);
                _viewModel.LoadItemsForMonth(_viewModel.SelectedDate);

                await DisplayAlert("Добавлено", $"«{newItem.Title}» добавлено в список", "ОК");
            }

            await Navigation.PopAsync();
        }
    }
}