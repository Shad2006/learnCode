using System.Collections.Generic;
using System.Windows;
using LearnCode;
using System;
using System.Windows.Media;
using System.Linq;
using System.Windows.Controls;
namespace LearnCodeWPF
{
    public partial class TeacherMainWindow : Window
    {
        private ServerDatabase db;
        private TeacherServer server;
        public TeacherMainWindow()
        {
            InitializeComponent();
            db = new ServerDatabase();
            LoadStatistics();
        }
        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            var fio = (sender as Button)?.Tag as string;
            if (MessageBox.Show($"Удалить ученика {fio} и весь его прогресс?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                db.DeleteStudent(fio);
                LoadStatistics();
            }
        }
        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var fio = (sender as Button)?.Tag as string;
            var dlg = new Window { Title = "Смена пароля", Width = 300, Height = 150, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            var panel = new StackPanel();
            var tb = new TextBox { Margin = new Thickness(5) };
            var btn = new Button { Content = "Сохранить", Margin = new Thickness(5) };
            btn.Click += (s, ev) => { db.ChangeStudentPassword(fio, tb.Text); dlg.Close(); LoadStatistics(); };
            panel.Children.Add(new TextBlock { Text = $"Новый пароль для {fio}:" });
            panel.Children.Add(tb);
            panel.Children.Add(btn);
            dlg.Content = panel;
            dlg.ShowDialog();
        }
        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                server = new TeacherServer();
                server.Start();
                serverStatusText.Text = "Запущен";
                serverStatusText.Foreground = Brushes.Green;
                btnStartServer.IsEnabled = false;
                btnStopServer.IsEnabled = true;
                string ip = GetLocalIpAddress();
                serverIpLabel.Text = $"IP: {ip}:8080";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска сервера: {ex.Message}");
            }
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            server?.Stop();
            server = null;
            serverStatusText.Text = "Остановлен";
            serverStatusText.Foreground = Brushes.Red;
            btnStartServer.IsEnabled = true;
            btnStopServer.IsEnabled = false;
            serverIpLabel.Text = "IP: не определен";
        }

        private string GetLocalIpAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }
        private void LoadStatistics()
        {
            try
            {
                var records = db.GetAllProgressRecords();
                dgStudents.ItemsSource = records;
                if (records.Count == 0)
                    MessageBox.Show("Нет записей прогресса. Ученики ещё не проходили тесты.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}");
            }
        }
        private void BtnEditContent_Click(object sender, RoutedEventArgs e)
        {
            TeacherEditWindow editWin = new TeacherEditWindow();
            editWin.ShowDialog();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {

            RegWindow wind = new RegWindow();
            wind.ShowDialog();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow wind = new StartWindow();
            wind.Show();
            this.Close();
        }
        private void ViewAnswers(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int? selected = btn?.Tag as int?;
            if (selected == null)
            {
                MessageBox.Show("Не удалось определить выбранную строку.");
                return;
            }
            var win = new TeacherAnswers(selected.Value);
            win.ShowDialog();
        }
        public class StudentStat
        {
            public string StudentName { get; set; }
            public string Course { get; set; }
            public int CompletedLessons { get; set; }
            public int AverageScore { get; set; }
        }
    }
}