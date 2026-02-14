using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;

namespace LearnCodeWPF
{
    public partial class QuizWindow : Window
    {
        private int currentQuestion = 0;
        private int score = 0;
        private DataRow[] questions;
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

            if (currentQuestion >= questions.Length)
            {
                FinishTest();
                return;
            }

            DataRow question = questions[currentQuestion];
            txtQuestionNumber.Text = $"Вопрос {currentQuestion + 1}";
            txtQuestion.Text = question["question_text"].ToString();
            string questionType = question["question_type"].ToString();
            string optionsText = question["options"].ToString();

            if (questionType == "single")
            {
                txtInstruction.Text = "Выберите ответ.";
                string[] options = optionsText.Split('|');
                foreach (var option in options)
                {
                    var radio = new RadioButton
                    {
                        Content = option,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    singleChoiceButtons.Add(radio);
                    optionsPanel.Children.Add(radio);
                }
            }
            else if (questionType == "multiple")
            {
                txtInstruction.Text = "Выберите ответ.";
                string[] options = optionsText.Split('|');
                foreach (var option in options)
                {
                    var checkBox = new CheckBox
                    {
                        Content = option,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    multipleChoiceBoxes.Add(checkBox);
                    optionsPanel.Children.Add(checkBox);
                }
            }
            else if (questionType == "text")
            {
                txtInstruction.Text = "Введите ответ.";
                textAnswer = new TextBox
                {
                    FontSize = 16,
                    Height = 60,
                    Width = 700,
                    TextAlignment = TextAlignment.Center,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1)
                };
                optionsPanel.Children.Add(textAnswer);
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuestion++;
            LoadQuestion();
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow question = questions[currentQuestion];
            string correctAnswer = question["correct_answer"].ToString();
            string questionType = question["question_type"].ToString();
            bool isCorrect = false;

            if (questionType == "single")
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
            else if (questionType == "multiple")
            {
                string selected = "";
                foreach (var checkBox in multipleChoiceBoxes)
                {
                    if (checkBox.IsChecked == true)
                    {
                        selected += checkBox.Content.ToString() + ",";
                    }
                }
                selected = selected.TrimEnd(',');
                if (selected == correctAnswer)
                {
                    isCorrect = true;
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
                Database db = new Database();
                db.UpdateUserExperience(10);
            }
            else
            {
                ShowResultMessage($"Неправильно. Правильный ответ: {correctAnswer}", false);
            }

            currentQuestion++;
            LoadQuestion();
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
            db.UnlockLesson(lessonNumber + 1, course);

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
                Text = $"Ваш результат: {score} из {questions.Length * 10}",
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
        }
    }
}