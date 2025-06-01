using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp;
using MyFirstMauiApp.Views;

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
                Title = "�������������� ����";
                titleEntry.Text = _editingItem.Title;
                descriptionEditor.Text = _editingItem.Description;
                categoryPicker.SelectedItem = _editingItem.Category;
                datePicker.Date = _editingItem.Date;
                timePicker.Time = _editingItem.Time;
            }
            else
            {
                Title = "����� ����";
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
                _editingItem.Time = timePicker.Time;

                await _viewModel.SaveAll(); // ��������� ��� ���������
                _viewModel.LoadItemsForDate(_viewModel.SelectedDate);

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
                    Time = timePicker.Time,
                    IsDone = false
                };

                _viewModel.SaveItem(newItem);
                _viewModel.LoadItemsForDate(_viewModel.SelectedDate);

                await DisplayAlert("���������", $"�{newItem.Title}� ��������� � ������", "��");
            }

            await Navigation.PopAsync();
        }
    }
}