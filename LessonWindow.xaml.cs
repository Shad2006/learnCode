using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
namespace LearnCodeWPF
{
    public partial class LessonWindow : Window
    {
        private int lessonNumber;
        private bool useServer;
        private string course;
        private Database db;
        public string userName;
        public LessonWindow(int lessonNum, string crs, string username, bool useServ)
        {
            InitializeComponent();
            lessonNumber = lessonNum;
            course = crs;
            userName = username;
            useServer = useServ;
            LoadLesson();
        }
        private async void LoadLesson()
        {
            Lesson lesson = null;
            if (useServer)
            {
                var lessons = await ServerSync.FetchLessonsAsync(course);
                if (lessons != null)
                    lesson = lessons.FirstOrDefault(l => l.LessonNumber == lessonNumber);
            }
            else
            {
                Database db = new Database();
                var lessons = db.GetLessons(course);
                lesson = lessons.FirstOrDefault(l => l.LessonNumber == lessonNumber);
            }
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
            bool isserver;
            if (userName != "Хакер") { isserver = true; } else { isserver = false; }
            QuizWindow quiz = new QuizWindow(lessonNumber, course, userName, isserver);
            quiz.Show();
            this.Close();}
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            bool isserver;
            if (userName != "Хакер") {  isserver = true; } else { isserver = false; }
            CourseWindow courseWindow = new CourseWindow(course, userName, isserver);
            courseWindow.Show();
            this.Close();
        }

        private async void ExplainSimpler_Click(object sender, RoutedEventArgs e)
        {
            Lesson currentLesson = null;
            if (useServer)
            {
                var lessons = await ServerSync.FetchLessonsAsync(course);
                currentLesson = lessons?.FirstOrDefault(l => l.LessonNumber == lessonNumber);
            }
            else
            {
                Database db = new Database();
                var lessons = db.GetLessons(course);
                currentLesson = lessons.FirstOrDefault(l => l.LessonNumber == lessonNumber);
            }
            if (currentLesson != null)
            {
                var explanationWindow = new ExplanationWindow(currentLesson.LessonName, currentLesson.TheoryText, course);
                explanationWindow.Owner = this;
                explanationWindow.ShowDialog();
            }
        }
    }
}