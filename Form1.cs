using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class Form1 : Form
{
    Panel panelMain = new Panel();
    PictureBox pictureLogo = new PictureBox();
    Label labelTitle = new Label();
    Label labelSubtitle = new Label();
    LinkLabel register = new LinkLabel();
    Panel panelCard = new Panel();
    TextBox textName = new TextBox();
    TextBox textPassword = new TextBox();
    Panel panelLevel = new Panel();
    RadioButton[] levelButtons;
    Panel panelCourse = new Panel();
    RadioButton[] courseButtons;
    Button buttonStart = new Button();
    string selectedCourse = "";
    string selectedLevel = "";
    public Form1()
    {
        this.Text = "LearnCode - обучение программированию";
        this.Size = new Size(900, 800);
        this.BackgroundImage = Image.FromFile("background.png");
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        panelMain.Dock = DockStyle.Fill;
        panelMain.BackgroundImage = Image.FromFile("background.png");
        labelTitle.Text = "Изучите один из языков программирования сегодня";
        labelTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
        labelTitle.BackColor = Color.FromArgb(0, 0, 0, 0);
        labelTitle.TextAlign = ContentAlignment.MiddleCenter;
        labelTitle.Size = new Size(500, 50);
        labelTitle.Location = new Point(200, 130);
        labelSubtitle.Text = "Давай познакомимся!";
        labelSubtitle.Font = new Font("Segoe UI", 14);
        labelSubtitle.BackColor = Color.Transparent;
        labelSubtitle.ForeColor = Color.Black;
        labelSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        labelSubtitle.Size = new Size(500, 30);
        labelSubtitle.Location = new Point(200, 190);
        panelCard.BackColor = Color.White;
        panelCard.Size = new Size(600, 400);
        panelCard.Location = new Point(150, 240);
        RoundPanel(panelCard, 20);
        Label labelName = new Label();
        labelName.Text = "Как тебя зовут?";
        labelName.Font = new Font("Segoe UI", 13);
        labelName.ForeColor = Color.FromArgb(80, 80, 80);
        labelName.Size = new Size(250, 25);
        labelName.Location = new Point(40, 30);
        textName.BorderStyle = BorderStyle.None;
        textName.Font = new Font("Segoe UI", 13);
        textName.Size = new Size(520, 35);
        textName.Location = new Point(40, 60);
        textName.BackColor = Color.FromArgb(250, 250, 250);
        RoundTextBox(textName, 10);
        textName.Padding = new Padding(10, 8, 10, 8);
        Label labelLevelTitle = new Label();
        labelLevelTitle.Text = "Определи уровень: Насколько хорошо знаешь язык:";
        labelLevelTitle.Font = new Font("Segoe UI", 13);
        labelLevelTitle.ForeColor = Color.FromArgb(80, 80, 80);
        labelLevelTitle.Size = new Size(250, 25);
        labelLevelTitle.Location = new Point(40, 115);

        panelLevel.Size = new Size(520, 50);
        panelLevel.Location = new Point(40, 145);
        panelLevel.BackColor = Color.Transparent;

        string[] levels = { "Начинающий", "Средний", "Продвинутый" };
        levelButtons = new RadioButton[levels.Length];
        int levelX = 0;
        for (int i = 0; i < levels.Length; i++)
        {
            levelButtons[i] = CreateLevelButton(levels[i], levelX);
            levelButtons[i].Tag = levels[i];
            panelLevel.Controls.Add(levelButtons[i]);
            levelX += 170;
        }
        Label labelCourseTitle = new Label();
        labelCourseTitle.Text = "Выбери язык программирования:";
        labelCourseTitle.Font = new Font("Segoe UI", 13);
        labelCourseTitle.ForeColor = Color.FromArgb(80, 80, 80);
        labelCourseTitle.Size = new Size(250, 25);
        labelCourseTitle.Location = new Point(40, 210);
        panelCourse.Size = new Size(520, 100);
        panelCourse.Location = new Point(40, 240);
        panelCourse.BackColor = Color.Transparent;
        string[] courses = { "csharp", "cpp", "php" };
        courseButtons = new RadioButton[courses.Length];
        int courseX = 0;
        int courseY = 0;
        for (int i = 0; i < courses.Length; i++)
        {
            courseButtons[i] = CreateCourseButton(courses[i], courseX, courseY);
            courseButtons[i].Tag = courses[i];
            panelCourse.Controls.Add(courseButtons[i]);

            courseX += 170;
            if (courseX >= 520)
            {
                courseX = 0;
                courseY += 50;
            }
        }
        buttonStart.Text = "Начать учится!";
        buttonStart.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        buttonStart.BackColor = Color.FromArgb(58, 179, 104);
        buttonStart.ForeColor = Color.White;
        buttonStart.FlatStyle = FlatStyle.Flat;
        buttonStart.FlatAppearance.BorderSize = 0;
        buttonStart.Size = new Size(520, 55);
        buttonStart.Location = new Point(40, 340);
        buttonStart.Cursor = Cursors.Hand;
        RoundButton(buttonStart, 12);
        buttonStart.MouseEnter += (s, e) =>
        {
            buttonStart.BackColor = Color.FromArgb(68, 189, 114);
            buttonStart.ForeColor = Color.White;
        };
        buttonStart.MouseLeave += (s, e) =>
        {
            buttonStart.BackColor = Color.FromArgb(58, 179, 104);
            buttonStart.ForeColor = Color.White;
        };

        buttonStart.Click += (s, e) =>
        {
            if (string.IsNullOrEmpty(textName.Text) ||
                string.IsNullOrEmpty(selectedLevel) ||
                string.IsNullOrEmpty(selectedCourse))
            {
                ShowMessage("Please fill in all fields!", Color.FromArgb(255, 87, 87));
                return;
            }

            ShowMessage($"Starting {selectedCourse} course for {selectedLevel}!", Color.FromArgb(58, 179, 104));
            Database2 db = new Database2();
            db.AddUser(textName.Text, selectedLevel, selectedCourse);
            Form2 form2 = new Form2(selectedCourse);
            form2.Show();
            this.Hide();
        };
        panelCard.Controls.Add(labelName);
        panelCard.Controls.Add(textName);
        panelCard.Controls.Add(labelLevelTitle);
        panelCard.Controls.Add(panelLevel);
        panelCard.Controls.Add(labelCourseTitle);
        panelCard.Controls.Add(panelCourse);
        panelCard.Controls.Add(buttonStart);
        panelMain.Controls.Add(pictureLogo);
        panelMain.Controls.Add(labelTitle);
        panelMain.Controls.Add(labelSubtitle);
        panelMain.Controls.Add(panelCard);

        this.Controls.Add(panelMain);
    }

    private RadioButton CreateLevelButton(string text, int x)
    {
        RadioButton radio = new RadioButton();
        radio.Text = text;
        radio.Font = new Font("Segoe UI", 11);
        radio.Size = new Size(160, 45);
        radio.Location = new Point(x, 0);
        radio.Appearance = Appearance.Button;
        radio.FlatStyle = FlatStyle.Flat;
        radio.TextAlign = ContentAlignment.MiddleCenter;
        radio.BackColor = Color.FromArgb(245, 245, 245);
        radio.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
        radio.FlatAppearance.BorderSize = 1;
        RoundButton(radio, 8);

        radio.CheckedChanged += (s, e) =>
        {
            if (radio.Checked)
            {
                selectedLevel = radio.Tag.ToString();
                radio.BackColor = Color.FromArgb(230, 245, 238);
                radio.FlatAppearance.BorderColor = Color.FromArgb(58, 179, 104);
                radio.ForeColor = Color.FromArgb(58, 179, 104);
            }
            else
            {
                radio.BackColor = Color.FromArgb(245, 245, 245);
                radio.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                radio.ForeColor = Color.FromArgb(80, 80, 80);
            }
        };

        return radio;
    }

    private RadioButton CreateCourseButton(string course, int x, int y)
    {
        RadioButton radio = new RadioButton();
        radio.Text = course;
        radio.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        radio.Size = new Size(80, 80);
        radio.Location = new Point(x, y);
        radio.Appearance = Appearance.Button;
        radio.BackgroundImage = Image.FromFile(course + ".png");
        radio.FlatStyle = FlatStyle.Flat;
        radio.TextAlign = ContentAlignment.MiddleCenter;
        radio.BackColor = Color.FromArgb(245, 245, 245);
        radio.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
        radio.FlatAppearance.BorderSize = 1;
        RoundButton(radio, 8);

        radio.CheckedChanged += (s, e) =>
        {
            if (radio.Checked)
            {
                selectedCourse = radio.Tag.ToString();
                radio.BackColor = Color.FromArgb(230, 245, 238);
                radio.FlatAppearance.BorderColor = Color.FromArgb(58, 179, 104);
                radio.ForeColor = Color.FromArgb(58, 179, 104);
            }
            else
            {
                radio.BackColor = Color.FromArgb(245, 245, 245);
                radio.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                radio.ForeColor = Color.FromArgb(80, 80, 80);
            }
        };

        return radio;
    }

    private void RoundButton(Control control, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
        path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
        path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
        path.CloseFigure();
        control.Region = new Region(path);
    }

    private void RoundPanel(Control control, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
        path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
        path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
        path.CloseFigure();
        control.Region = new Region(path);
    }
    private void RoundTextBox(Control control, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
        path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
        path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
        path.CloseFigure();
        control.Region = new Region(path);
    }
    private void ShowMessage(string text, Color color)
    {
        Label message = new Label();
        message.Text = text;
        message.Font = new Font("Segoe UI", 12);
        message.ForeColor = color;
        message.BackColor = Color.White;
        message.TextAlign = ContentAlignment.MiddleCenter;
        message.Size = new Size(520, 40);
        message.Location = new Point(40, 425);
        message.BorderStyle = BorderStyle.FixedSingle;
        RoundPanel(message, 8);

        panelCard.Controls.Add(message);

        Timer timer = new Timer();
        timer.Interval = 3000;
        timer.Tick += (s, e) =>
        {
            panelCard.Controls.Remove(message);
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }
}