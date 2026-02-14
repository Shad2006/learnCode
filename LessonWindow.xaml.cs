using System.Windows;
using System.Windows.Controls;

namespace LearnCodeWPF
{
    public partial class LessonWindow : Window
    {
        private int lessonNumber;
        private string course;

        public string LessonTitle => $"Урок {lessonNumber}";

        public LessonWindow(int number, string crs)
        {
            lessonNumber = number;
            course = crs;
            InitializeComponent();
            DataContext = this;
            LoadTheory();
        }

        private void LoadTheory()
        {
            Database db = new Database();
            var lessons = db.GetLessons(course);

            foreach (var lesson in lessons)
            {
                if ((int)lesson["lesson_number"] == lessonNumber)
                {
                    txtTheory.Text = lesson["theory_text"].ToString();
                    break;
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow courseWindow = new CourseWindow(course);
            courseWindow.Show();
            this.Close();
        }

        private void GuideButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Справочник по {course}\n\nЗдесь будет справочная информация по языку {course}.",
                "Справочник",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow(lessonNumber, course);
            quizWindow.Show();
            this.Close();
        }
    }
}