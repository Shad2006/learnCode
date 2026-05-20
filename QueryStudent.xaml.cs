using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LearnCodeWPF;
using LearnCodeUWP;
namespace LearnCode
{
    /// <summary>
    /// Interaction logic for QueryStudent.xaml
    /// </summary>
    public partial class QueryStudent : Window
    {
        public Database db;
        public QueryStudent()
        {
            InitializeComponent();
             db = new Database();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = loginfield.Text;
            string password = passwordfield.Text;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            bool success = db.Auth(login, password);
            if (success)
            {
                SelectLanguageWindow langWindow = new SelectLanguageWindow(login);
                langWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
