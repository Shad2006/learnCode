using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
namespace LearnCodeWPF
{
    public partial class CourseWindow : Window
    {
        private string course;
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
        public ObservableCollection<LessonViewModel> Lessons { get; set; }
        public string CourseTitle { get; set; }
        public string UserFIO { get; set; } = "Хакер";
        public string ProgressWidth { get; set; } = "";
        public string ProgressPercent { get; set; } = "";
        public string ProgressDetails { get; set; } = "";
        public CourseWindow(string selectedCourse, string userName, bool useServ)
        {
            course = selectedCourse;
            CourseTitle = course;
            Lessons = new ObservableCollection<LessonViewModel>();
            InitializeComponent();
            DataContext = this;
            if (userName != "")
            {
                UserFIO = userName;
            }
            LoadData();
            
        }
        private async void LoadData()
        {
            List<Lesson> lessonsData = null;
            string userName = UserFIO;
            bool useServer = !string.IsNullOrEmpty(userName) && userName != "Хакер";
            if (useServer)
            {
                lessonsData = await ServerSync.FetchLessonsAsync(course);
                if (lessonsData != null)
                {
                    var completedLessons = await ServerSync.FetchCompletedLessonsAsync(userName, course);
                    foreach (var lesson in lessonsData)
                    {
                        bool unlocked = (lesson.LessonNumber == 1) ||
                                        (completedLessons != null && completedLessons.Contains(lesson.LessonNumber - 1));
                        Lessons.Add(new LessonViewModel
                        {
                            LessonNumber = lesson.LessonNumber,
                            LessonName = lesson.LessonName,
                            IsLocked = !unlocked,
                            Color = unlocked ? new SolidColorBrush(lessonColors[lesson.LessonNumber % lessonColors.Length]) : Brushes.Gray,
                            TextColor = Brushes.Black,
                            Icon = GetProgrammingIcon(lesson.LessonName)
                        });
                    }
                    return;
                }
            }
            Database db = new Database();
            lessonsData = db.GetLessons(course);
            foreach (var lesson in lessonsData)
            {
                Lessons.Add(new LessonViewModel
                {
                    LessonNumber = lesson.LessonNumber,
                    LessonName = lesson.LessonName,
                    IsLocked = lesson.IsLocked,
                    Color = lesson.IsLocked ? Brushes.Gray : new SolidColorBrush(lessonColors[lesson.LessonNumber % lessonColors.Length]),
                    TextColor = Brushes.Black,
                    Icon = GetProgrammingIcon(lesson.LessonName)
                });
            }
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
            StartWindow win = new StartWindow();
            win.Show();
            this.Close();
        }
        private void Projects_Click(object sender, MouseButtonEventArgs e)
        {
            ProjectsWindow proj = new ProjectsWindow();
            proj.Show();
            this.Close();
        }
        private void LessonItem_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var lesson = border?.DataContext as LessonViewModel;
            if (lesson.IsLocked == true) { }
            else
            {
                if (lesson != null)
                {
                    bool useServer = !string.IsNullOrEmpty(UserFIO) && UserFIO != "Хакер";
                    LessonWindow lessonWindow = new LessonWindow(lesson.LessonNumber, course, UserFIO, useServer);
                    lessonWindow.Show();
                    this.Close();
                }
            }
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
            if (Lessons.Count > 0)
            {
                bool useServer = !string.IsNullOrEmpty(UserFIO) && UserFIO != "Хакер";
                int lessonNumber = Lessons[0].LessonNumber;
                LessonWindow lessonWindow = new LessonWindow(lessonNumber, course, UserFIO, useServer);
                lessonWindow.Show();
                this.Close();
            }
        }
        private void LessonItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var lesson = border?.DataContext as LessonViewModel;

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