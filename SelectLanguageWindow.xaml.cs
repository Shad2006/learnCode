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
        private bool servermode;
        public SelectLanguageWindow(string login, bool serverMode)
        {
            InitializeComponent();
            user = login;
            servermode = serverMode;
                
    }
        private void Select_php(object sender, RoutedEventArgs e)
        {
            CourseWindow php = new CourseWindow("PHP", user, servermode);
            php.Show();
            this.Close();
        }
        private void Csharp_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow scharp = new CourseWindow("C#", user,  servermode);
            scharp.Show();
            this.Close();
        }
        private void Cpp_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow scharp = new CourseWindow("C++", user,  servermode);
            scharp.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow scharp = new CourseWindow("C++", user,  servermode);
            scharp.Show();
            this.Close();
        }
    }
}
