using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp;
using MyFirstMauiApp.Views;
using Plugin.LocalNotification;

namespace MyFirstMauiApp.Views
{
    public partial class AddEditPage : ContentPage
    {
        private ScheduleViewModel _viewModel;
        private ScheduleItem _editingItem;

        public AddEditPage(ScheduleViewModel viewModel, ScheduleItem editingItem = null)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _editingItem = editingItem;

            if (_editingItem != null)
            {
                Title = "Редактирование задачи";
                titleEntry.Text = _editingItem.Title;
                descriptionEditor.Text = _editingItem.Description;
                categoryPicker.SelectedItem = _editingItem.Category;
                datePicker.Date = _editingItem.Date;

                // Если есть время включаем чекбокс и заполняем TimePicker
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
            else
            {
                Title = "Новая задача";

                // По умолчанию чекбокс выключен, TimePicker отключен
                timeEnabledCheckBox.IsChecked = false;
                timePicker.IsEnabled = false;
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
                _editingItem.Time = timeEnabledCheckBox.IsChecked == true ? timePicker.Time : (TimeSpan?)null;

                await _viewModel.SaveAll(); // сохраняем все изменения
                _viewModel.LoadItemsForDate(_viewModel.SelectedDate);

                // Уведомление (если есть время)
                if (_editingItem.Time != null)
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
                            NotifyTime = notifyDateTime,
                            NotifyRepeatInterval = null
                        }
                    };

                    LocalNotificationCenter.Current.Show(notification);
                }

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
                    Time = timeEnabledCheckBox.IsChecked == true ? timePicker.Time : (TimeSpan?)null,
                    IsDone = false
                };

                _viewModel.SaveItem(newItem);
                _viewModel.LoadItemsForDate(_viewModel.SelectedDate);

                // Уведомление (если есть время)
                if (newItem.Time != null)
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
                            NotifyTime = notifyDateTime,
                            NotifyRepeatInterval = null
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