using Microsoft.Maui.Controls;
using System;

namespace MyFirstMauiApp.Views
{
    public partial class TaskDetailsMonthPage : ContentPage
    {
        public TaskDetailsMonthPage(ScheduleItem item)
        {
            InitializeComponent();
            BindingContext = item;
        }
    }
}