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
using LearnCode;
using LearnCodeWPF;
using LearnCodeUWP;
namespace LearnCodeUWP
{
    /// <summary>
    /// Interaction logic for SelectLanguageWindow.xaml
    /// </summary>
    public partial class SelectLanguageWindow : Window
    {
        private string user;
        public SelectLanguageWindow(string login)
        {
            InitializeComponent();
            user = login;
    }
        private void Select_php(object sender, RoutedEventArgs e)
        {
            CourseWindow php = new CourseWindow("PHP", user);
            php.Show();
            this.Close();
        }
        private void Csharp_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow scharp = new CourseWindow("C#", user);
            scharp.Show();
            this.Close();
        }
        private void Cpp_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow scharp = new CourseWindow("C++", user);
            scharp.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow scharp = new CourseWindow("C++", user);
            scharp.Show();
            this.Close();
        }
    }
}
