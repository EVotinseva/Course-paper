using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp;
using Plugin.LocalNotification;

namespace MyFirstMauiApp.Views
{
    public partial class AddEditPageMonth : ContentPage
    {
        private MonthViewModel _viewModel;
        private ScheduleItem _editingItem;
        private bool _isFreeTask;

        public AddEditPageMonth(MonthViewModel viewModel, ScheduleItem editingItem = null, bool isFreeTask = false)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _editingItem = editingItem;
            _isFreeTask = isFreeTask;

            if (_editingItem != null)
            {
                Title = "Редактирование задачи";
                titleEntry.Text = _editingItem.Title;
                descriptionEditor.Text = _editingItem.Description;
                categoryPicker.SelectedItem = _editingItem.Category;

                if (_editingItem.Date == DateTime.MinValue)
                {
                    dateTimeSelection.IsVisible = false;
                }
                else
                {
                    dateTimeSelection.IsVisible = true;
                    datePicker.Date = _editingItem.Date;

                    if (_editingItem.Time != null)
                    {
                        timeEnabledCheckBox.IsChecked = true;
                        timePicker.Time = _editingItem.Time.Value;
                        timePicker.IsEnabled = true;
                    }
                    else
                    {
                        timeEnabledCheckBox.IsChecked = false;
                        timePicker.IsEnabled = false;
                    }
                }
            }
            else
            {
                Title = "Новая задача";

                if (_isFreeTask)
                {
                    dateTimeSelection.IsVisible = false;
                    datePicker.Date = DateTime.MinValue;
                }
                else
                {
                    dateTimeSelection.IsVisible = true;
                    datePicker.Date = DateTime.Today;
                }

                timeEnabledCheckBox.IsChecked = false;
                timePicker.IsEnabled = false;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_editingItem != null)
            {
                _editingItem.Title = titleEntry.Text;
                _editingItem.Description = descriptionEditor.Text;
                _editingItem.Category = categoryPicker.SelectedItem?.ToString() ?? "Без категории";
                _editingItem.Date = _isFreeTask ? DateTime.MinValue : datePicker.Date;
                _editingItem.Time = (_isFreeTask || !timeEnabledCheckBox.IsChecked) ? null : timePicker.Time;

                await _viewModel.SaveAll();
                _viewModel.LoadItemsForMonth(_viewModel.SelectedDate);

                if (_editingItem.Time != null && _editingItem.Date != DateTime.MinValue)
                {
                    var notifyDateTime = new DateTime(
                        _editingItem.Date.Year,
                        _editingItem.Date.Month,
                        _editingItem.Date.Day,
                        _editingItem.Time.Value.Hours,
                        _editingItem.Time.Value.Minutes,
                        0);

                    var notification = new NotificationRequest
                    {
                        NotificationId = new Random().Next(1000, 9999),
                        Title = "Напоминание",
                        Description = _editingItem.Title,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notifyDateTime
                        }
                    };

                    LocalNotificationCenter.Current.Show(notification);
                }

                await DisplayAlert("Изменено", $"«{_editingItem.Title}» обновлено", "ОК");
            }
            else
            {
                var newItem = new ScheduleItem
                {
                    Title = titleEntry.Text,
                    Description = descriptionEditor.Text,
                    Category = categoryPicker.SelectedItem?.ToString() ?? "Без категории",
                    Date = _isFreeTask ? DateTime.MinValue : datePicker.Date,
                    Time = (_isFreeTask || !timeEnabledCheckBox.IsChecked) ? null : timePicker.Time,
                    IsDone = false,
                    IsMonthlyFree = _isFreeTask
                };

                _viewModel.SaveItem(newItem);
                _viewModel.LoadItemsForMonth(_viewModel.SelectedDate);

                if (newItem.Time != null && newItem.Date != DateTime.MinValue)
                {
                    var notifyDateTime = new DateTime(
                        newItem.Date.Year,
                        newItem.Date.Month,
                        newItem.Date.Day,
                        newItem.Time.Value.Hours,
                        newItem.Time.Value.Minutes,
                        0);

                    var notification = new NotificationRequest
                    {
                        NotificationId = new Random().Next(1000, 9999),
                        Title = "Напоминание",
                        Description = newItem.Title,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notifyDateTime
                        }
                    };

                    LocalNotificationCenter.Current.Show(notification);
                }

                await DisplayAlert("Добавлено", $"«{newItem.Title}» добавлено в список", "ОК");
            }

            await Navigation.PopAsync();
        }

        private void OnTimeEnabledChanged(object sender, CheckedChangedEventArgs e)
        {
            timePicker.IsEnabled = e.Value;
        }
    }
}