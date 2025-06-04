using Microsoft.Maui.Controls;
using System;
using MyFirstMauiApp.Views;

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

        // ? ��������� ������ ����
        public void OnMonthStartSelected(object sender, DateChangedEventArgs e)
        {
            ViewModel.LoadItemsForMonth(e.NewDate.Date);
        }

        // ? ������� � ���������� ������ ����
        public async void OnAddClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPageMonth(ViewModel));
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
                await Navigation.PushAsync(new AddEditPageMonth(ViewModel, item));
            }
        }

        public async void OnTaskTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is ScheduleItem item)
            {
                await Navigation.PushAsync(new TaskDetailsMonthPage(item));
            }
        }
    }
}