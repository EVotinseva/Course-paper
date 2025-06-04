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

        // ? ��������� ������ ����
        public void OnWeekStartSelected(object sender, DateChangedEventArgs e)
        {
            ViewModel.LoadItemsForWeek(e.NewDate.Date);
        }

        // ? ������� � ���������� ������ ����
        public async void OnAddClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPageWeek(ViewModel));
        }

        // ? ������� ����������
        public void OnCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox cb && cb.BindingContext is ScheduleItem item)
            {
                ViewModel.ToggleDone(item, e.Value);
            }
        }

        // ? �������� ������
        public async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is ScheduleItem item)
            {
                bool confirm = await DisplayAlert("�������", $"������� ���� �{item.Title}�?", "��", "���");
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
    }
}