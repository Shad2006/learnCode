using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LearnCodeWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                ShowMessage("Please enter your name!", Colors.Red);
                return;
            }

            string selectedLevel = "";
            if (rbBeginner.IsChecked == true) selectedLevel = "Начинающий";
            else if (rbIntermediate.IsChecked == true) selectedLevel = "Средний";
            else if (rbAdvanced.IsChecked == true) selectedLevel = "Продвинутый";

            string selectedCourse = "";
            if (rbCSharp.IsChecked == true) selectedCourse = "C#";
            else if (rbCPP.IsChecked == true) selectedCourse = "C++";
            else if (rbPHP.IsChecked == true) selectedCourse = "PHP";

            if (string.IsNullOrEmpty(selectedLevel) || string.IsNullOrEmpty(selectedCourse))
            {
                ShowMessage("Please select level and course!", Colors.Red);
                return;
            }

            Database db = new Database();
            db.AddUser(txtName.Text, selectedLevel, selectedCourse);

            CourseWindow courseWindow = new CourseWindow(selectedCourse);
            courseWindow.Show();
            this.Close();
        }

        private void ShowMessage(string text, Color color)
        {
            var message = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(color),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(40, 10, 40, 0),
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = new SolidColorBrush(color),
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(10)
                }
            };

            var grid = (Grid)Content;
            grid.Children.Add(message);
        }
    }
}