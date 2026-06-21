using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LearnCode;
namespace LearnCodeWPF
{
    public class QuestionAnswerDisplay
    {
        public int Number { get; set; }
        public string QuestionText { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public Brush AnswerColor { get; set; }
    }
    public partial class TeacherAnswers : Window
    {
        public TeacherAnswers(int id)
        {
            InitializeComponent();
            LoadData(id);
        }

        private async void LoadData(int id)
        {
            ServerDatabase db = new ServerDatabase();
            var (studentName, course, lessonNumber) = db.GetProgressDetails(id);
            var answersDict = await ServerSync.FetchAnswersAsync(id);
            var questions = new ServerDatabase().GetQuestions(lessonNumber, course);
            var displayList = new List<QuestionAnswerDisplay>();
            for (int i = 0; i < questions.Count; i++)
            {
                var q = questions[i];
                string userAnswer = answersDict != null? answersDict[i] : "(нет ответа)";
                bool isCorrect = (userAnswer == q.CorrectAnswer);
                displayList.Add(new QuestionAnswerDisplay
                {
                    Number = i + 1,
                    QuestionText = q.QuestionText,
                    UserAnswer = userAnswer,
                    CorrectAnswer = q.CorrectAnswer,
                    IsCorrect = isCorrect,
                    AnswerColor = isCorrect ? Brushes.Green : Brushes.Red
                });
            }
            answersList.ItemsSource = displayList;
        }
    }
}