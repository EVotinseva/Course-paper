using Microsoft.Maui.Controls;
using System;

namespace MyFirstMauiApp.Views
{
    public partial class TaskDetailsPage : ContentPage
    {
        public TaskDetailsPage(ScheduleItem item)
        {
            InitializeComponent();
            BindingContext = item;
        }
    }
}