using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
namespace LearnCodeWPF
{
    public partial class CourseWindow : Window
    {
        private string course;
        private List<LessonViewModel> lessons = new List<LessonViewModel>();
        private Color[] lessonColors = new Color[]
        {
            Color.FromRgb(88, 204, 2),
            Color.FromRgb(28, 176, 246),
            Color.FromRgb(206, 130, 255),
            Color.FromRgb(255, 150, 0),
            Color.FromRgb(0, 205, 156),
            Color.FromRgb(255, 134, 208),
            Color.FromRgb(255, 75, 75),
            Color.FromRgb(255, 200, 0)
        };
        public CourseWindow(string selectedCourse)
        {
            course = selectedCourse;
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            Database db = new Database();
            var lessonsData = db.GetLessons(course);

            foreach (var lesson in lessonsData)
            {
                bool isLocked = (bool)lesson["is_locked"];
                lessons.Add(new LessonViewModel
                {
                    LessonNumber = (int)lesson["lesson_number"],
                    LessonName = lesson["lesson_name"].ToString(),
                    IsLocked = isLocked,
                    Color = new SolidColorBrush(lessonColors[(int)lesson["lesson_number"] % lessonColors.Length]),
                    TextColor = isLocked ? Brushes.Gray : Brushes.Black,
                    Icon = GetProgrammingIcon(lesson["lesson_name"].ToString())
                });
            }

            LessonsList.ItemsSource = lessons;
        }

        private string GetProgrammingIcon(string lessonName)
        {
            string lowerName = lessonName.ToLower();
            if (lowerName.Contains("введение") || lowerName.Contains("основы")) return "📚";
            else if (lowerName.Contains("переменн")) return "𝑥";
            else if (lowerName.Contains("тип")) return "𝕋";
            else if (lowerName.Contains("оператор")) return "+−×÷";
            else if (lowerName.Contains("услов")) return "?";
            else if (lowerName.Contains("цикл")) return "⟳";
            else if (lowerName.Contains("функци")) return "ƒ()";
            else if (lowerName.Contains("массив")) return "[]";
            else if (lowerName.Contains("объект")) return "{}";
            else if (lowerName.Contains("класс")) return "𝐂";
            else if (lowerName.Contains("алгоритм")) return "⚙";
            else if (lowerName.Contains("ошибк")) return "⚠";
            else if (lowerName.Contains("практик")) return "💻";
            else return "λ";
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void NavItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(247, 247, 247));
            }
        }

        private void NavItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = Brushes.Transparent;
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Database db = new Database();
            var lessonsData = db.GetLessons(course);
            foreach (var lesson in lessonsData)
            {
                if (!(bool)lesson["is_locked"])
                {
                    int lessonNumber = (int)lesson["lesson_number"];
                    LessonWindow lessonWindow = new LessonWindow(lessonNumber, course);
                    lessonWindow.Show();
                    this.Close();
                    return;
                }
            }

            if (lessonsData.Length > 0)
            {
                LessonWindow lessonWindow = new LessonWindow(1, course);
                lessonWindow.Show();
                this.Close();
            }
        }

        private void LessonItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var lesson = border.DataContext as LessonViewModel;

            if (border != null && lesson != null && !lesson.IsLocked)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(247, 247, 247));
            }
        }

        private void LessonItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = Brushes.Transparent;
            }
        }
    }

    public class LessonViewModel
    {
        public int LessonNumber { get; set; }
        public string LessonName { get; set; }
        public bool IsLocked { get; set; }
        public Brush Color { get; set; }
        public Brush TextColor { get; set; }
        public string Icon { get; set; }
        public string LessonNumberText => $"УРОК {LessonNumber}";
    }
}