using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LearnCodeWPF
{
    public partial class TeacherEditWindow : Window
    {
        private Database db;
        private List<Lesson> allLessons;
        private List<Question> allQuestions;
        private Lesson currentLesson;
        private Question currentQuestion;
        private string currentCourse;
        private int currentLessonNumber;

        public TeacherEditWindow()
        {
            InitializeComponent();
            db = new Database();
            LoadCourses();
        }

        private void LoadCourses()
        {
            cmbCourse.SelectedIndex = 0;
        }

        private void CmbCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCourse.SelectedItem is ComboBoxItem item)
                currentCourse = item.Content.ToString();
            LoadLessons();
        }

        private void LoadLessons()
        {
            allLessons = db.GetLessons(currentCourse);
            cmbLesson.ItemsSource = allLessons;
            cmbLesson.DisplayMemberPath = "LessonName";
            cmbLesson.SelectedValuePath = "LessonNumber";
            if (allLessons.Count > 0)
                cmbLesson.SelectedIndex = 0;
        }

        private void CmbLesson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbLesson.SelectedItem is Lesson lesson)
            {
                currentLesson = lesson;
                if (currentLesson != null)
                {
                    txtLessonName.Text = currentLesson.LessonName;
                    currentLessonNumber = lesson.LessonNumber;
                    chkLocked.IsChecked = currentLesson.IsLocked;
                    LoadHtmlForEditing(currentLesson.TheoryText ?? "");
                    LoadQuestions();
                }
            }
        }

        private void LoadQuestions()
        {
            allQuestions = db.GetQuestions(currentLessonNumber, currentCourse);
            lbQuestions.ItemsSource = allQuestions;
            if (allQuestions.Count > 0)
                lbQuestions.SelectedIndex = 0;
            else
                ClearQuestionForm();
        }

        private void BtnNewLesson_Click(object sender, RoutedEventArgs e)
        {
            int newNumber = allLessons.Count + 1;
            currentLesson = new Lesson
            {
                LessonNumber = newNumber,
                LessonName = "Новый урок",
                TheoryText = "",
                IsLocked = true
            };
            db.AddLesson(currentLesson.LessonNumber, currentLesson.LessonName, currentCourse, currentLesson.TheoryText, currentLesson.IsLocked);
            LoadLessons();
            LoadQuestions();
            cmbLesson.SelectedItem = currentLesson;
        }

        private void LbQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbQuestions.SelectedItem is Question q)
            {
                currentQuestion = q;
                txtQuestionText.Text = q.QuestionText;
                txtCorrectAnswer.Text = q.CorrectAnswer;
                txtOptions.Text = q.Options?.Replace("|", "\n") ?? "";
                string typeEn = q.QuestionType;
                string typeRu = "";
                if (typeEn == "choice") typeRu = "Одиночный выбор";
                else if (typeEn == "multiple") typeRu = "Множественный выбор";
                else if (typeEn == "text") typeRu = "Текстовый ответ";
                else if (typeEn == "code") typeRu = "Программный код";
                cmbQuestionType.SelectedItem = cmbQuestionType.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == typeRu);
                //UpdateHint();
            }
        }

        private void BtnNewQuestion_Click(object sender, RoutedEventArgs e)
        {
            ClearQuestionForm();
            currentQuestion = null;
        }

        private void ClearQuestionForm()
        {
            txtQuestionText.Text = "";
            txtCorrectAnswer.Text = "";
            txtOptions.Text = "";
            cmbQuestionType.SelectedIndex = 0;
        }

        private void CmbQuestionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            UpdateHint();
        }

        private void UpdateHint()
        {
            string typeRu = (cmbQuestionType.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (typeRu == "Программный код")
                lblHint.Text = "Опишите задание подробно. В правильный ответ введите ожидаемый вывод программы.";
            else if (typeRu == "Одиночный выбор" || typeRu == "Множественный выбор")
                lblHint.Text = "Варианты ответов: каждый на новой строке. Правильный ответ должен точно совпадать с одним из вариантов (для множественного выбора перечислите через |, например: ответ1|ответ2)";
            else
                lblHint.Text = "Введите точный текстовый ответ.";
        }
        */
        }
        private void BtnSaveQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (currentLesson == null)
            {
                MessageBox.Show("Сначала выберите или создайте урок");
                return;
            }
            string typeRu = (cmbQuestionType.SelectedItem as ComboBoxItem).Content.ToString();
            string typeEn = "";
            if (typeRu == "Одиночный выбор") typeEn = "choice";
            else if (typeRu == "Множественный выбор") typeEn = "multiply";
            else if (typeRu == "Текстовый ответ") typeEn = "text";
            else if (typeRu == "Программный код") typeEn = "code";
            currentLessonNumber = currentLesson.LessonNumber;
            string options = txtOptions.Text.Replace("\r\n", "|").Replace("\n", "|");
            string correct = txtCorrectAnswer.Text.Trim();
            string qtext = txtQuestionText.Text.Trim();
            if (string.IsNullOrEmpty(qtext))
            {
                MessageBox.Show("Введите текст вопроса");
                return;
            }
            
            else
            {
            }

            db.AddQuestion(currentLesson.LessonNumber, currentCourse, qtext, typeEn, correct, options);
            LoadQuestions();
            ClearQuestionForm();
        }
        private string GetEditedHtml()
        {
            try
            {
                object result = webBrowser.Document.InvokeScript("getHtml");
                return result?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }
        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            string command = (sender as Button).Tag.ToString();
            webBrowser.Document.InvokeScript("execCmd", new object[] { command, null });
                webBrowser.Document.Focus();
            
        }

        private void ColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbColor.SelectedItem is ComboBoxItem item)
            {
                string color = item.Tag.ToString();
                try
                {
                    webBrowser.Document.InvokeScript("execCmd", new object[] { "forecolor", color });
                    webBrowser.Document.Focus();
                }
                catch { }
            }
        }
        private void CodeButton_Click(object sender, RoutedEventArgs e) {
            webBrowser.Document.InvokeScript("execCommand", new object[] { "insertHTML", false, "<code>вcтавьте программный код сюда</code>" });
        }
        private void BtnSaveLesson_Click(object sender, RoutedEventArgs e)
        {
            if (currentLesson == null) return;
            currentLesson.LessonName = txtLessonName.Text;
            currentLesson.TheoryText = GetEditedHtml();
            currentLesson.IsLocked = chkLocked.IsChecked == true;
            db.UpdateLesson(currentLesson, currentCourse);
            MessageBox.Show("Урок сохранён");
        }
        private void LoadHtmlForEditing(string htmlContent)
        {
            string fullHtml = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8'>
        <style>
            body {{ font-family: 'Segoe UI', sans-serif; margin: 10px; }}
            .editor {{ min-height: 200px; border: 1px solid #ccc; padding: 8px; }}
        </style>
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
        <script>
            function getHtml() {{
                return document.getElementById('editor').innerHTML;
            }}
            function execCmd(command, value) {{
                document.execCommand(command, false, value);
            }}
        </script>
    </head>
    <body>
        <div id='editor' class='editor' contenteditable='true'>{htmlContent}</div>
    </body>
    </html>";

            webBrowser.DocumentText = fullHtml;
        }
        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}