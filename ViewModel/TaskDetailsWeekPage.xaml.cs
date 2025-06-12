using Microsoft.Maui.Controls;
using System;

namespace MyFirstMauiApp.Views
{
    public partial class TaskDetailsWeekPage : ContentPage
    {
        private ScheduleItem _item;

        public TaskDetailsWeekPage(ScheduleItem item)
        {
            InitializeComponent();
            _item = item;
            BindingContext = _item;

            // Скрыть дату и время, если задача свободная (Date == MinValue)
            bool isFree = _item.Date == DateTime.MinValue;

            dateSection.IsVisible = !isFree;
            timeSection.IsVisible = !isFree && _item.Time != null;
        }
    }
}