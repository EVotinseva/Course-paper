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
                Title = "�������������� ������";
                titleEntry.Text = _editingItem.Title;
                descriptionEditor.Text = _editingItem.Description;
                categoryPicker.SelectedItem = _editingItem.Category;
                datePicker.Date = _editingItem.Date;

                // ���� ���� ����� �������� ������� � ��������� TimePicker
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
                Title = "����� ������";

                // �� ��������� ������� ��������, TimePicker ��������
                timeEnabledCheckBox.IsChecked = false;
                timePicker.IsEnabled = false;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_editingItem != null)
            {
                // ����� ��������������
                _editingItem.Title = titleEntry.Text;
                _editingItem.Description = descriptionEditor.Text;
                _editingItem.Category = categoryPicker.SelectedItem?.ToString() ?? "��� ���������";
                _editingItem.Date = datePicker.Date;
                _editingItem.Time = timeEnabledCheckBox.IsChecked == true ? timePicker.Time : (TimeSpan?)null;

                await _viewModel.SaveAll(); // ��������� ��� ���������
                _viewModel.LoadItemsForDate(_viewModel.SelectedDate);

                // ����������� (���� ���� �����)
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
                        Title = "�����������",
                        Description = _editingItem.Title,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notifyDateTime,
                            NotifyRepeatInterval = null
                        }
                    };

                    LocalNotificationCenter.Current.Show(notification);
                }

                await DisplayAlert("��������", $"�{_editingItem.Title}� ���������", "��");
            }
            else
            {
                // ����� �������� ����� ������
                var newItem = new ScheduleItem
                {
                    Title = titleEntry.Text,
                    Description = descriptionEditor.Text,
                    Category = categoryPicker.SelectedItem?.ToString() ?? "��� ���������",
                    Date = datePicker.Date,
                    Time = timeEnabledCheckBox.IsChecked == true ? timePicker.Time : (TimeSpan?)null,
                    IsDone = false
                };

                _viewModel.SaveItem(newItem);
                _viewModel.LoadItemsForDate(_viewModel.SelectedDate);

                // ����������� (���� ���� �����)
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
                        Title = "�����������",
                        Description = newItem.Title,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notifyDateTime,
                            NotifyRepeatInterval = null
                        }
                    };

                    LocalNotificationCenter.Current.Show(notification);
                }

                await DisplayAlert("���������", $"�{newItem.Title}� ��������� � ������", "��");
            }

            await Navigation.PopAsync();
        }

        private void OnTimeEnabledChanged(object sender, CheckedChangedEventArgs e)
        {
            timePicker.IsEnabled = e.Value;
        }
    }
}