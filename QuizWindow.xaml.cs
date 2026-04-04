using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace LearnCodeWPF
{
    public partial class QuizWindow : Window
    {
        private int currentQuestion = 0;
        private int score = 0;
        private string course;
        private int lessonNumber;
        private List<RadioButton> singleChoiceButtons = new List<RadioButton>();
        private List<CheckBox> multipleChoiceBoxes = new List<CheckBox>();
        private TextBox textAnswer;
        public QuizWindow(int lessonNum, string crs)
        {
            lessonNumber = lessonNum;
            course = crs;
            InitializeComponent();
            LoadQuestions();
            LoadQuestion();
        }
        private List<Question> questions; 
        private void LoadQuestions()
        {
            Database db = new Database();
            questions = db.GetQuestions(lessonNumber, course);
        }
        private void LoadQuestion()
        {
            optionsPanel.Children.Clear();
            singleChoiceButtons.Clear();
            multipleChoiceBoxes.Clear();
            if (questions == null || questions.Count == 0)
            {
                FinishTest();
                return;
            }
            if (currentQuestion >= questions.Count)
            {
                FinishTest();
                return;
            }
            var question = questions[currentQuestion];
            txtQuestionNumber.Text = $"Вопрос {currentQuestion + 1}";
            txtQuestion.Text = question.QuestionText;
            if (question.QuestionType == "choice")
            {txtInstruction.Text = "Выберите ответ.";
                string[] options = question.Options.Split('|');
                foreach (var option in options)
                {
                    var radio = new RadioButton
                    {Content = option,
                        FontSize = 24,
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    singleChoiceButtons.Add(radio);
                    optionsPanel.Children.Add(radio);
                }
            }
            else if (question.QuestionType == "text")
            {
                txtInstruction.Text = "Введите ответ.";
                textAnswer = new TextBox
                {
                    FontSize = 20,
                    Height = 60,
                    Width = 700,
                    TextAlignment = TextAlignment.Center,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1)
                };
                optionsPanel.Children.Add(textAnswer);
            }
            else if (question.QuestionType == "code")
            {
                txtInstruction.Text = "Напишите код, решающий задачу.";
                var codeEditor = new TextBox
                {
                    AcceptsReturn = true,
                    AcceptsTab = true,
                    FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                    FontSize = 24,
                    Height = 200,
                    TextWrapping = TextWrapping.NoWrap,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                optionsPanel.Children.Add(codeEditor);
                textAnswer = codeEditor;
                var runButton = new Button { Content = "Запустить код", Margin = new Thickness(0, 10, 0, 0) };
                runButton.Click += async (senderObj, eventArgs) => await RunCodeAndCheck(codeEditor.Text);
                optionsPanel.Children.Add(runButton);
            }
        }
        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuestion++;
            LoadQuestion();
        }
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            var question = questions[currentQuestion];
            string correctAnswer = question.CorrectAnswer;
            string questionType = question.QuestionType;
            bool isCorrect = false;
            if (questionType == "choice")
            {
                foreach (var radio in singleChoiceButtons)
                {
                    if (radio.IsChecked == true && radio.Content.ToString() == correctAnswer)
                    {
                        isCorrect = true;
                        break;
                    }
                }
            }
            else if (questionType == "text")
            {
                if (textAnswer.Text.Trim().ToLower() == correctAnswer.ToLower())
                {
                    isCorrect = true;
                }
            }
            if (isCorrect)
            {
                score += 10;
                ShowResultMessage("Правильно! +10 опыта", true);
            }
            else
            {
                ShowResultMessage($"Неправильно. Правильный ответ: {correctAnswer}", false);
            }
            currentQuestion++;
            LoadQuestion();
        }
        private async Task RunCodeAndCheck(string userCode)
        {
            var question = questions[currentQuestion];
            string expected = question.CorrectAnswer.Trim();
            string actual = await CodeRunner.RunCodeAsync(userCode, course); // course = "C#"/"C++"/"PHP"
            if (actual == expected)
            {
                score += 10;
                ShowResultMessage("Правильно! Код выдал верный вывод.", true);
                currentQuestion++;
                LoadQuestion();
            }
            else
            {ShowResultMessage($"Неправильно. Ваш вывод:\n{actual}\n\nОжидалось:\n{expected}", false);
            }
        }
        private void ShowResultMessage(string message, bool isSuccess)
        {
            var resultWindow = new Window
            {
                Title = isSuccess ? "Отлично!" : "Результат",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White
            };
            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var label = new TextBlock
            {
                Text = message,
                FontSize = 12,
                Foreground = isSuccess ? Brushes.Green : Brushes.Red,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(20)
            };
            var button = new Button
            {
                Content = "OK",
                Background = Brushes.Green,
                Foreground = Brushes.White,
                Width = 100,
                Height = 40,
                Margin = new Thickness(0, 20, 0, 0)
            };
            button.Click += (s, e) => resultWindow.Close();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(button);
            resultWindow.Content = stackPanel;
            resultWindow.ShowDialog();
        }
        private void FinishTest()
        {
            Database db = new Database();
            var resultWindow = new Window
            {
                Title = "Тест завершен",
                Width = 500,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White
            };
            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var label1 = new TextBlock
            {
                Text = "Тест завершен!",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Green,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            var label2 = new TextBlock
            {
                Text = $"Ваш результат: {score} из {questions.Count * 10}",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            var label3 = new TextBlock
            {
                Text = "Следующий урок разблокирован!",
                FontSize = 12,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            };
            var button = new Button
            {
                Content = "Вернуться к урокам",
                Background = Brushes.Green,
                Foreground = Brushes.White,
                Width = 200,
                Height = 50,
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };
            button.Click += (s, e) =>
            {
                resultWindow.Close();
                CourseWindow courseWindow = new CourseWindow(course);
                courseWindow.Show();
                this.Close();
            };
            stackPanel.Children.Add(label1);
            stackPanel.Children.Add(label2);
            stackPanel.Children.Add(label3);
            stackPanel.Children.Add(button);
            resultWindow.Content = stackPanel;
            resultWindow.ShowDialog();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LessonWindow lessonWindow = new LessonWindow(lessonNumber, course);
            lessonWindow.Show();
            this.Close();
        }}}