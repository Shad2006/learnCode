using System.Windows;
using LearnCodeWPF;
namespace LearnCodeWPF
{
    public partial class TeacherLogin : Window
    {
        public TeacherLogin()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string password = pwdBox.Password;
            if (password == "teacher123")
            {
                TeacherMainWindow mainWin = new TeacherMainWindow();
                mainWin.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}