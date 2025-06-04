using Microsoft.Maui.Controls;
using System;

namespace MyFirstMauiApp.Views
{
    public partial class TaskDetailsWeekPage : ContentPage
    {
        public TaskDetailsWeekPage(ScheduleItem item)
        {
            InitializeComponent();
            BindingContext = item;
        }
    }
}