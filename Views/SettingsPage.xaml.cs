using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyFirstMauiApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void OnOpenManualClicked(object sender, EventArgs e)
        {
            await OpenUserManualPdf();
        }

        private async Task OpenUserManualPdf()
        {
            var fileName = "user_manual.pdf";
            var targetPath = Path.Combine(FileSystem.CacheDirectory, fileName);

            if (!File.Exists(targetPath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                using var outStream = File.Create(targetPath);
                await stream.CopyToAsync(outStream);
            }

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(targetPath),
                Title = "Открыть руководство пользователя"
            });
        }
    }
}