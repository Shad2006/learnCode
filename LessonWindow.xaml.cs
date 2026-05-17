using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
namespace LearnCodeWPF
{
    public partial class LessonWindow : Window
    {
        private int lessonNumber;
        private string course;
        private Database db;
        public string userName;
        public LessonWindow(int lessonNum, string crs, string username)
        {
            InitializeComponent();
            lessonNumber = lessonNum;
            course = crs;
            db = new Database();
            LoadLesson();
            userName = username;
        }
        private void LoadLesson()
        {
            var lessons = db.GetLessons(course);
            var lesson = lessons.FirstOrDefault(l => l.LessonNumber == lessonNumber);
            if (lesson != null)
            {
                txtLessonTitle.Text = lesson.LessonName;
                string htmlContent = GenerateHtml(lesson.TheoryText, lesson.LessonName);
                webBrowser.DocumentText = htmlContent;
            }
            else
            {
                txtLessonTitle.Text = "Урок не найден";
            }
        }
        private string GenerateHtml(string theoryText, string lessonTitle)
        {
            string html = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <title>{lessonTitle}</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    margin: 20px;
                    line-height: 1.6;
                    background-color: #F7F7F7;
                    color: #333;
                }}
                pre {{
                    background-color: #2D2D2D;
                    color: #F8F8F2;
                    padding: 10px;
                    border-radius: 5px;
                    overflow-x: auto;
                }}
                code {{
                    font-family: 'Consolas', monospace;
                    font-size: 14px;
                }}
                h1 {{
                    color: #58CC02;
                }}
                .example {{
                    background-color: #E8F5E9;
                    padding: 10px;
                    border-left: 4px solid #58CC02;
                }}
            </style>
        </head>
        <body>
            {theoryText}
        </body>
        </html>";
            return html;
        }
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quiz = new QuizWindow(lessonNumber, course, userName);
            quiz.Show();
            this.Close();}
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CourseWindow courseWindow = new CourseWindow(course, userName);
            courseWindow.Show();
            this.Close();
        }

        private void ExplainSimpler_Click(object sender, RoutedEventArgs e)
        {
            var lessons = db.GetLessons(course);
            var currentLesson = lessons.FirstOrDefault(l => l.LessonNumber == lessonNumber);
            if (currentLesson != null)
            {
                var explanationWindow = new ExplanationWindow(currentLesson.LessonName, currentLesson.TheoryText, course);
                explanationWindow.Owner = this;
                explanationWindow.ShowDialog();
            }
            else
            {
            }
        }
    }
}