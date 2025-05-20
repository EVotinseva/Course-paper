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

        // ? ��������� ������ ����
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            ViewModel.LoadItemsForDate(e.NewDate.Date);
        }

        // ? ������� � ���������� ������ ����
        private async void OnAddClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPage(ViewModel));
        }

        // ? ������� ����������
        private void OnCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox cb && cb.BindingContext is ScheduleItem item)
            {
                ViewModel.ToggleDone(item, e.Value);
            }
        }

        // ? �������� ������
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is ScheduleItem item)
            {
                bool confirm = await DisplayAlert("�������", $"������� ���� �{item.Title}�?", "��", "���");
                if (confirm)
                {
                    ViewModel.DeleteItem(item);
                }
            }
        }
    }
}