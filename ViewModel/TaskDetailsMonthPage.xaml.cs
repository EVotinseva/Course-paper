using Microsoft.Maui.Controls;
using System;

namespace MyFirstMauiApp.Views
{
    public partial class TaskDetailsMonthPage : ContentPage
    {
        private ScheduleItem _item;

        public TaskDetailsMonthPage(ScheduleItem item)
        {
            InitializeComponent();
            _item = item;
            BindingContext = _item;

            bool isFree = _item.Date == DateTime.MinValue;

            dateSection.IsVisible = !isFree;
            timeSection.IsVisible = !isFree && _item.Time != null;
        }
    }
}