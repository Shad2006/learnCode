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
namespace LearnCode
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }
        public void btnStart_Click(object sender, RoutedEventArgs e)
        {
            SelectLanguageWindow newwindow = new SelectLanguageWindow();
            newwindow.Show();
            this.Close();
        }
        public void btn_login_Click(object sender, RoutedEventArgs e)
        {
            RegWindow newwindow = new RegWindow();
            newwindow.Show();
            this.Close();
        }
    }
}
