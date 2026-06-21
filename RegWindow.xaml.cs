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
namespace LearnCode
{
    /// <summary>
    /// Interaction logic for RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        private ServerDatabase db;
        private string login;
        private string password;
        public RegWindow()
        {
                InitializeComponent();
            db = new ServerDatabase();
        }
        public void registration()
        {
            string FIO = loginfield.Text;
            string cod = passwordfield.Text;
            db.AddStudent(FIO, cod);
            MessageBox.Show("Ученик зарегестрирован", "Ученик зарегестрирован", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            registration();
        }
    }
}
