using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LearnCodeWPF
{
    public partial class TeacherWindow : Window
    {
        private Database db;
        private ObservableCollection<Lesson> lessons;
        private ObservableCollection<Question> questions;
        private Lesson currentLesson;
        private Question currentQuestion;
        private string currentCourseLessons = "C#";
        private string currentCourseQuestions = "C#";

        public TeacherWindow()
        {
            InitializeComponent();
            db = new Database();
            LoadLessons(currentCourseLessons);
            LoadQuestions(currentCourseQuestions, "");
        }

        private void LoadLessons(string course)
        {
            var list = db.GetLessons(course);
            lessons = new ObservableCollection<Lesson>(list);
            dgLessons.ItemsSource = lessons;
        }

        private void LoadQuestions(string course, string lessonNumberFilter)
        {
            var all = db.GetAllQuestions();
            var filtered = all.Where(q => q.Course == course).ToList();
            if (int.TryParse(lessonNumberFilter, out int lessonNum))
                filtered = filtered.Where(q => q.LessonNumber == lessonNum).ToList();
            questions = new ObservableCollection<Question>(filtered);
            dgQuestions.ItemsSource = questions;
        }
        private void CmbCourseLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCourseLessons.SelectedItem is ComboBoxItem item)
            {
                currentCourseLessons = item.Content.ToString();
                LoadLessons(currentCourseLessons);
            }
        }

        private void DgLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLesson = dgLessons.SelectedItem as Lesson;
            if (currentLesson != null)
            {
                txtLessonNumber.Text = currentLesson.LessonNumber.ToString();
                txtLessonName.Text = currentLesson.LessonName;
                txtTheoryText.Text = currentLesson.TheoryText;
                chkIsLocked.IsChecked = currentLesson.IsLocked;
            }
        }

        private void AddLesson_Click(object sender, RoutedEventArgs e)
        {
            int newNumber = lessons.Count + 1;
            var newLesson = new Lesson
            {
                LessonNumber = newNumber,
                LessonName = "Новый урок",
                TheoryText = "",
                IsLocked = true,
                Course = currentCourseLessons
            };
            db.AddLessonComplete(newLesson, currentCourseLessons);
            LoadLessons(currentCourseLessons);
        }

        private void EditLesson_Click(object sender, RoutedEventArgs e)
        {
            var lesson = (sender as Button)?.Tag as Lesson;
            if (lesson != null)
            {
                txtLessonNumber.Text = lesson.LessonNumber.ToString();
                txtLessonName.Text = lesson.LessonName;
                txtTheoryText.Text = lesson.TheoryText;
                chkIsLocked.IsChecked = lesson.IsLocked;
                currentLesson = lesson;
            }
        }

        private void DeleteLesson_Click(object sender, RoutedEventArgs e)
        {
            var lesson = (sender as Button)?.Tag as Lesson;
            if (lesson != null && MessageBox.Show($"Удалить урок '{lesson.LessonName}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                db.DeleteLesson(lesson.LessonNumber, lesson.Course);
                LoadLessons(currentCourseLessons);
            }
        }

        private void SaveLessonChanges_Click(object sender, RoutedEventArgs e)
        {
            if (currentLesson == null)
            {
                MessageBox.Show("Выберите урок для сохранения.");
                return;
            }
            if (!int.TryParse(txtLessonNumber.Text, out int num))
            {
                MessageBox.Show("Номер урока должен быть числом.");
                return;
            }
            currentLesson.LessonNumber = num;
            currentLesson.LessonName = txtLessonName.Text;
            currentLesson.TheoryText = txtTheoryText.Text;
            currentLesson.IsLocked = chkIsLocked.IsChecked == true;
            db.UpdateLesson(currentLesson, currentLesson.Course);
            LoadLessons(currentCourseLessons);
            MessageBox.Show("Урок сохранён");
        }
        private void CmbCourseQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCourseQuestions.SelectedItem is ComboBoxItem item)
            {
                currentCourseQuestions = item.Content.ToString();
                LoadQuestions(currentCourseQuestions, txtLessonNumberFilter.Text);
            }
        }

        private void TxtLessonNumberFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadQuestions(currentCourseQuestions, txtLessonNumberFilter.Text);
        }

        private void DgQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentQuestion = dgQuestions.SelectedItem as Question;
            if (currentQuestion != null)
            {
                txtQLessonNumber.Text = currentQuestion.LessonNumber.ToString();
                txtQText.Text = currentQuestion.QuestionText;
                cmbQType.SelectedItem = cmbQType.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == currentQuestion.QuestionType);
                txtQCorrect.Text = currentQuestion.CorrectAnswer;
                txtQOptions.Text = currentQuestion.Options;
            }
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            int newLessonNum = 1;
            var newQuestion = new Question
            {
                LessonNumber = newLessonNum,
                Course = currentCourseQuestions,
                QuestionText = "Новый вопрос",
                QuestionType = "text",
                CorrectAnswer = "",
                Options = ""
            };
            db.AddQuestionComplete(newQuestion, currentCourseQuestions);
            LoadQuestions(currentCourseQuestions, txtLessonNumberFilter.Text);
        }

        private void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            var question = (sender as Button)?.Tag as Question;
            if (question != null)
            {
                txtQLessonNumber.Text = question.LessonNumber.ToString();
                txtQText.Text = question.QuestionText;
                cmbQType.SelectedItem = cmbQType.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == question.QuestionType);
                txtQCorrect.Text = question.CorrectAnswer;
                txtQOptions.Text = question.Options;
                currentQuestion = question;
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            var question = (sender as Button)?.Tag as Question;
            if (question != null && MessageBox.Show($"Удалить вопрос '{question.QuestionText}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                db.DeleteQuestion(question.LessonNumber, question.Course, question.QuestionText);
                LoadQuestions(currentCourseQuestions, txtLessonNumberFilter.Text);
            }
        }

        private void SaveQuestionChanges_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion == null)
            {
                MessageBox.Show("Выберите вопрос для сохранения.");
                return;
            }
            if (!int.TryParse(txtQLessonNumber.Text, out int lessonNum))
            {
                MessageBox.Show("Номер урока должен быть числом.");
                return;
            }
            currentQuestion.LessonNumber = lessonNum;
            currentQuestion.QuestionText = txtQText.Text;
            currentQuestion.QuestionType = (cmbQType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "text";
            currentQuestion.CorrectAnswer = txtQCorrect.Text;
            currentQuestion.Options = txtQOptions.Text;
            LoadQuestions(currentCourseQuestions, txtLessonNumberFilter.Text);
            MessageBox.Show("Вопрос сохранён");
        }

        private void ExitTeacherMode_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void InsertTag_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button)?.Tag as string;
            if (string.IsNullOrEmpty(tag)) return;
            string openTag = $"<{tag}>";
            string closeTag = $"</{tag}>";
            int start = txtTheoryText.SelectionStart;
            string selected = txtTheoryText.SelectedText;
            if (string.IsNullOrEmpty(selected))
            {
                txtTheoryText.Text = txtTheoryText.Text.Insert(start, openTag + closeTag);
                txtTheoryText.SelectionStart = start + openTag.Length;
                txtTheoryText.SelectionLength = 0;
            }
            else
            {
                string newText = txtTheoryText.Text.Remove(start, selected.Length);
                newText = newText.Insert(start, openTag + selected + closeTag);
                txtTheoryText.Text = newText;
                txtTheoryText.SelectionStart = start + openTag.Length;
                txtTheoryText.SelectionLength = selected.Length;
            }
            txtTheoryText.Focus();
        }
        private void InsertPreCode_Click(object sender, RoutedEventArgs e)
        {
            string codeBlock = "<pre><code>\n\n</code></pre>";
            int start = txtTheoryText.SelectionStart;
            txtTheoryText.Text = txtTheoryText.Text.Insert(start, codeBlock);
            txtTheoryText.SelectionStart = start + 12;
            txtTheoryText.Focus();
        }
        private void InsertList_Click(object sender, RoutedEventArgs e)
        {
            string listBlock = "<ul>\n  <li>Пункт 1</li>\n  <li>Пункт 2</li>\n</ul>";
            int start = txtTheoryText.SelectionStart;
            txtTheoryText.Text = txtTheoryText.Text.Insert(start, listBlock);
            txtTheoryText.SelectionStart = start + listBlock.Length;
            txtTheoryText.Focus();
        }
        private void TxtTheoryText_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }
        private void PreviewHtml_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            string html = txtTheoryText.Text;
            string fullHtml = $@"
    <html>
    <head>
        <meta charset='utf-8'>
        <style>
            body {{ font-family: 'Segoe UI', sans-serif; margin: 10px; line-height: 1.5; }}
            pre {{ background: #f4f4f4; padding: 8px; border-radius: 6px; overflow-x: auto; }}
            code {{ font-family: Consolas, monospace; }}
            h1 {{ color: #58CC02; }}
        </style>
    </head>
    <body>{html}</body>
    </html>";
        }
    }
}