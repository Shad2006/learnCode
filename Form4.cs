using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
public class Form4 : Form
{
    Panel panelMain = new Panel();
    Panel panelHeader = new Panel();
    Panel panelQuestion = new Panel();
    Panel panelOptions = new Panel();
    Panel panelControls = new Panel();
    Label labelInstruction = new Label();
    Label labelQuestionText = new Label();
    Button buttonCheck = new Button();
    Button buttonSkip = new Button();
    Button buttonBack = new Button();
    Label labelQuestionNumber = new Label();
    int currentQuestion = 0;
    int score = 0;
    DataRow[] questions;
    string course;
    int lessonNumber;
    TextBox textboxAnswer = new TextBox();
    List<Panel> optionCards = new List<Panel>();
    List<Label> optionLabels = new List<Label>();
    List<bool> selectedOptions = new List<bool>();
    bool isMultipleChoice = false;
    public Form4(int lessonNum, string crs)
    {
        lessonNumber = lessonNum;
        course = crs;
        this.Text = "Вопросы урока " + lessonNumber;
        this.Size = new Size(1200, 900);
        this.BackColor = Color.White;
        this.StartPosition = FormStartPosition.CenterScreen;
        panelHeader.BackColor = Color.White;
        panelHeader.Size = new Size(900, 60);
        panelHeader.Dock = DockStyle.Top;
        panelHeader.BorderStyle = BorderStyle.None;
        labelQuestionNumber.Text = $"Вопрос {currentQuestion + 1}";
        labelQuestionNumber.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        labelQuestionNumber.ForeColor = Color.Black;
        labelQuestionNumber.Size = new Size(300, 40);
        labelQuestionNumber.Location = new Point(300, 10);
        labelQuestionNumber.TextAlign = ContentAlignment.MiddleCenter;
        buttonBack.Text = "←";
        buttonBack.Font = new Font("Segoe UI", 20, FontStyle.Bold);
        buttonBack.FlatStyle = FlatStyle.Flat;
        buttonBack.FlatAppearance.BorderSize = 0;
        buttonBack.ForeColor = Color.FromArgb(100, 100, 100);
        buttonBack.BackColor = Color.Transparent;
        buttonBack.Size = new Size(50, 40);
        buttonBack.Location = new Point(20, 10);
        buttonBack.Click += (s, e) =>
        {
            Form3 form3 = new Form3(lessonNumber, course);
            form3.Show();
            this.Hide();
        };
        panelHeader.Controls.Add(labelQuestionNumber);
        panelHeader.Controls.Add(buttonBack);
        panelMain.Size = new Size(1200, 900);
        panelMain.Location = new Point(0, 60);
        panelMain.BackColor = Color.White;
        panelQuestion.Size = new Size(800, 200);
        panelQuestion.Location = new Point(50, 30);
        panelQuestion.BackColor = Color.White;
        panelQuestion.BorderStyle = BorderStyle.None;
        labelInstruction.Font = new Font("Segoe UI", 20, FontStyle.Bold);
        labelInstruction.ForeColor = Color.Black;
        labelInstruction.Size = new Size(750, 60);
        labelInstruction.Location = new Point(0, 20);
        labelInstruction.TextAlign = ContentAlignment.MiddleCenter;
        labelQuestionText.Font = new Font("Segoe UI", 16);
        labelQuestionText.ForeColor = Color.FromArgb(80, 80, 80);
        labelQuestionText.Size = new Size(750, 80);
        labelQuestionText.Location = new Point(0, 80);
        labelQuestionText.TextAlign = ContentAlignment.MiddleCenter;
        panelQuestion.Controls.Add(labelInstruction);
        panelQuestion.Controls.Add(labelQuestionText);
        panelOptions.Size = new Size(800, 250);
        panelOptions.Location = new Point(50, 250);
        panelOptions.BackColor = Color.White;
        panelControls.Size = new Size(800, 100);
        panelControls.Location = new Point(50, 520);
        panelControls.BackColor = Color.White;
        buttonSkip.Text = "Пропустить";
        buttonSkip.Font = new Font("Segoe UI", 14);
        buttonSkip.FlatStyle = FlatStyle.Flat;
        buttonSkip.FlatAppearance.BorderSize = 1;
        buttonSkip.FlatAppearance.BorderColor = Color.FromArgb(88, 179, 104);
        buttonSkip.ForeColor = Color.FromArgb(88, 179, 104);
        buttonSkip.BackColor = Color.White;
        buttonSkip.Size = new Size(180, 50);
        buttonSkip.Location = new Point(100, 25);
        buttonSkip.Cursor = Cursors.Hand;
        buttonSkip.Click += (s, e) =>
        {
            currentQuestion++;
            LoadQuestion();
        };
        buttonCheck.Text = "Проверить";
        buttonCheck.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        buttonCheck.BackColor = Color.FromArgb(88, 179, 104);
        buttonCheck.ForeColor = Color.White;
        buttonCheck.FlatStyle = FlatStyle.Flat;
        buttonCheck.FlatAppearance.BorderSize = 0;
        buttonCheck.Size = new Size(180, 50);
        buttonCheck.Location = new Point(520, 25);
        buttonCheck.Cursor = Cursors.Hand;
        buttonCheck.Click += ButtonCheck_Click;
        MakeButtonRound(buttonSkip);
        MakeButtonRound(buttonCheck);
        panelControls.Controls.Add(buttonSkip);
        panelControls.Controls.Add(buttonCheck);
        panelMain.Controls.Add(panelQuestion);
        panelMain.Controls.Add(panelOptions);
        panelMain.Controls.Add(panelControls);
        this.Controls.Add(panelHeader);
        this.Controls.Add(panelMain);
        LoadQuestions();
        LoadQuestion();
    }
    void LoadQuestions()
    {
        Database2 db = new Database2();
        questions = db.GetQuestions(lessonNumber, course);
    }
    void LoadQuestion()
    {
        ClearOptions();
        if (currentQuestion >= questions.Length)
        {
            FinishTest();
            return;
        }
        DataRow question = questions[currentQuestion];
        labelQuestionNumber.Text = $"Вопрос {currentQuestion + 1}";
        labelQuestionText.Text = question["question_text"].ToString();
        string questionType = question["question_type"].ToString();
        string optionsText = question["options"].ToString();
        if (questionType == "single")
        {
            labelInstruction.Text = "Выберите ответ.";
            isMultipleChoice = false;
            string[] options = optionsText.Split('|');
            CreateOptionCards(options, false);
        }
        else if (questionType == "multiple")
        {
            labelInstruction.Text = "Выберите ответ.";
            isMultipleChoice = true;
            string[] options = optionsText.Split('|');
            CreateOptionCards(options, true);
        }
        else if (questionType == "text")
        {
            labelInstruction.Text = "Введите ответ.";
            CreateTextInput();
        }
    }
    void CreateOptionCards(string[] options, bool allowMultiple)
    {
        int cardWidth = 300;
        int cardHeight = 200;
        int spacing = 15;
        int startX = 20;
        for (int i = 0; i < options.Length; i++)
        {
            string option = options[i];
            Panel card = new Panel();
            card.Size = new Size(cardWidth, cardHeight);
            card.Location = new Point(startX + i * (cardWidth + spacing), 20);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.Fixed3D;
            card.Cursor = Cursors.Hand;
            card.Tag = i;
            Label optionLabel = new Label();
            optionLabel.Text = options[i];
            optionLabel.Font = new Font("Segoe UI", 14);
            optionLabel.ForeColor = Color.Black;
            optionLabel.Size = new Size(650, 40);
            optionLabel.Location = new Point(20, 10);
            optionLabel.TextAlign = ContentAlignment.MiddleLeft;
            optionLabel.BackColor = Color.Transparent;
            card.Controls.Add(optionLabel);
            card.Click += (s, e) =>
            {
                int index = (int)card.Tag;
                if (allowMultiple)
                {
                    selectedOptions[index] = !selectedOptions[index];
                    card.BackColor = selectedOptions[index] ?
                        Color.FromArgb(230, 245, 230) : Color.White;
                    optionLabel.ForeColor = selectedOptions[index] ?
                        Color.FromArgb(88, 179, 104) : Color.Black;
                }
                else
                {
                    for (int j = 0; j < optionCards.Count; j++)
                    {
                        optionCards[j].BackColor = Color.White;
                        optionLabels[j].ForeColor = Color.Black;
                        selectedOptions[j] = false;
                    }
                    card.BackColor = Color.FromArgb(230, 245, 230);
                    optionLabel.ForeColor = Color.FromArgb(88, 179, 104);
                    selectedOptions[index] = true;
                }
            };
            optionCards.Add(card);
            optionLabels.Add(optionLabel);
            selectedOptions.Add(false);
            panelOptions.Controls.Add(card);
        }
    }
    void CreateTextInput()
    {
        textboxAnswer = new TextBox();
        textboxAnswer.Font = new Font("Segoe UI", 16);
        textboxAnswer.Size = new Size(700, 60);
        textboxAnswer.Location = new Point(50, 100);
        textboxAnswer.BorderStyle = BorderStyle.FixedSingle;
        textboxAnswer.BackColor = Color.White;
        textboxAnswer.TextAlign = HorizontalAlignment.Center;
        panelOptions.Controls.Add(textboxAnswer);
    }
    void ClearOptions()
    {
        panelOptions.Controls.Clear();
        optionCards.Clear();
        optionLabels.Clear();
        selectedOptions.Clear();
    }
    void ButtonCheck_Click(object sender, System.EventArgs e)
    {
        DataRow question = questions[currentQuestion];
        string correctAnswer = question["correct_answer"].ToString();
        string questionType = question["question_type"].ToString();
        bool isCorrect = false;
        if (questionType == "single")
        {
            for (int i = 0; i < selectedOptions.Count; i++)
            {
                if (selectedOptions[i] && optionLabels[i].Text == correctAnswer)
                {
                    isCorrect = true;
                    break;
                }
            }
        }
        else if (questionType == "multiple")
        {
            string selected = "";
            for (int i = 0; i < selectedOptions.Count; i++)
            {
                if (selectedOptions[i])
                {
                    selected += optionLabels[i].Text + ",";
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
            if (textboxAnswer.Text.Trim().ToLower() == correctAnswer.ToLower())
            {
                isCorrect = true;
            }
        }
        if (isCorrect)
        {
            score += 10;
            ShowResultMessage("Правильно! +10 опыта", true);
            Database2 db = new Database2();
            db.UpdateUserExperience(10);
        }
        else
        {
            ShowResultMessage($"Неправильно. Правильный ответ: {correctAnswer}", false);
        }
        currentQuestion++;
        LoadQuestion();
    }
    void ShowResultMessage(string message, bool isSuccess)
    {
        Form resultForm = new Form();
        resultForm.Text = isSuccess ? "Отлично!" : "Результат";
        resultForm.Size = new Size(400, 200);
        resultForm.StartPosition = FormStartPosition.CenterScreen;
        resultForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        resultForm.MaximizeBox = false;
        resultForm.MinimizeBox = false;
        resultForm.BackColor = Color.White;
        Label label = new Label();
        label.Text = message;
        label.Font = new Font("Segoe UI", 12);
        label.ForeColor = isSuccess ? Color.FromArgb(88, 179, 104) : Color.FromArgb(200, 50, 50);
        label.Size = new Size(350, 80);
        label.Location = new Point(25, 30);
        label.TextAlign = ContentAlignment.MiddleCenter;
        Button button = new Button();
        button.Text = "OK";
        button.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        button.BackColor = Color.FromArgb(88, 179, 104);
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Size = new Size(100, 40);
        button.Location = new Point(150, 100);
        button.Cursor = Cursors.Hand;
        MakeButtonRound(button);
        button.Click += (s, e) => resultForm.Close();
        resultForm.Controls.Add(label);
        resultForm.Controls.Add(button);
        resultForm.ShowDialog();
    }
    void FinishTest()
    {
        Database2 db = new Database2();
        db.UnlockLesson(lessonNumber + 1, course);
        Form resultForm = new Form();
        resultForm.Text = "Тест завершен";
        resultForm.Size = new Size(500, 300);
        resultForm.StartPosition = FormStartPosition.CenterScreen;
        resultForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        resultForm.MaximizeBox = false;
        resultForm.MinimizeBox = false;
        resultForm.BackColor = Color.White;
        Label label1 = new Label();
        label1.Text = "Тест завершен!";
        label1.Font = new Font("Segoe UI", 20, FontStyle.Bold);
        label1.ForeColor = Color.FromArgb(88, 179, 104);
        label1.Size = new Size(450, 60);
        label1.Location = new Point(25, 30);
        label1.TextAlign = ContentAlignment.MiddleCenter;
        Label label2 = new Label();
        label2.Text = $"Ваш результат: {score} из {questions.Length * 10}";
        label2.Font = new Font("Segoe UI", 14);
        label2.ForeColor = Color.Black;
        label2.Size = new Size(450, 40);
        label2.Location = new Point(25, 100);
        label2.TextAlign = ContentAlignment.MiddleCenter;
        Label label3 = new Label();
        label3.Text = "Следующий урок разблокирован!";
        label3.Font = new Font("Segoe UI", 12);
        label3.ForeColor = Color.FromArgb(100, 100, 100);
        label3.Size = new Size(450, 30);
        label3.Location = new Point(25, 150);
        label3.TextAlign = ContentAlignment.MiddleCenter;
        Button button = new Button();
        button.Text = "Вернуться к урокам";
        button.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        button.BackColor = Color.FromArgb(88, 179, 104);
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Size = new Size(200, 50);
        button.Location = new Point(150, 200);
        button.Cursor = Cursors.Hand;
        MakeButtonRound(button);
        button.Click += (s, e) =>
        {
            resultForm.Close();
            Form2 form2 = new Form2(course);
            form2.Show();
            this.Hide();
        };
        resultForm.Controls.Add(label1);
        resultForm.Controls.Add(label2);
        resultForm.Controls.Add(label3);
        resultForm.Controls.Add(button);
        resultForm.ShowDialog();
    }
    void MakeButtonRound(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        GraphicsPath path = new GraphicsPath();
        path.AddEllipse(0, 0, button.Width, button.Height);
        button.Region = new Region(path);
    }
}