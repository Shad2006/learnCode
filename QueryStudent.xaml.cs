using System;
using System.Threading.Tasks;
using System.Windows;
using LearnCodeWPF;
using LearnCodeUWP;

namespace LearnCode
{
    public partial class QueryStudent : Window
    {
        public QueryStudent()
        {
            InitializeComponent();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = loginfield.Text;
            string password = passwordfield.Text;
            string serverIp = serverIpField.Text.Trim();
            if (string.IsNullOrEmpty(serverIp))
                serverIp = "192.168.1.100";
            ServerSync.SetServerUrl($"http://{serverIp}:8080");
            bool success = await ServerSync.AuthAsync(login, password);
            if (success)
            {
                SelectLanguageWindow langWindow = new SelectLanguageWindow(login, serverMode: true);
                langWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль, или сервер недоступен.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            SelectLanguageWindow langWindow = new SelectLanguageWindow("Хакер", serverMode: false);
            langWindow.Show();
            this.Close();
        }
    }
}