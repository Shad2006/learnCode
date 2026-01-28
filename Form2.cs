using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Data;
using LearningApp;
public class Form2 : Form
{
    private readonly Color ColorSnow = Color.FromArgb(255, 255, 255);
    private readonly Color ColorPolar = Color.FromArgb(247, 247, 247);
    private readonly Color ColorSwan = Color.FromArgb(229, 229, 229);
    private readonly Color ColorHare = Color.FromArgb(175, 175, 175);
    private readonly Color ColorWolf = Color.FromArgb(119, 119, 119);
    private readonly Color ColorEel = Color.FromArgb(75, 75, 75);
    private readonly Color ColorMacaw = Color.FromArgb(28, 176, 246);
    private readonly Color ColorOwl = Color.FromArgb(88, 204, 2);
    private readonly Color ColorCardinal = Color.FromArgb(255, 75, 75);
    private readonly Color ColorBee = Color.FromArgb(255, 200, 0);
    private readonly Color ColorPeacock = Color.FromArgb(0, 205, 156);
    private readonly Color ColorBeetle = Color.FromArgb(206, 130, 255);
    private readonly Color ColorStarfish = Color.FromArgb(255, 134, 208);
    private readonly Color ColorFox = Color.FromArgb(255, 150, 0);
    private readonly Color ColorTreeFrog = Color.FromArgb(88, 167, 0);
    private readonly Color ColorTurtle = Color.FromArgb(165, 237, 110);
    private readonly Color ColorGuineaPig = Color.FromArgb(205, 121, 0);
    private Panel panelSidebar = new Panel();
    private Panel panelMain = new Panel();
    private Panel panelHeader = new Panel();
    private FlowLayoutPanel flowMain = new FlowLayoutPanel();
    private Panel panelPath = new Panel();
    private Panel panelDailyQuest = new Panel();
    private Panel panelStreak = new Panel();
    private Button buttonContinue = new Button();
    private string course;
    private string userFIO;
    private string userLevel;
    private Color[] lessonColors = new Color[]
    {
        Color.FromArgb(88, 204, 2),
        Color.FromArgb(28, 176, 246),
        Color.FromArgb(206, 130, 255),
        Color.FromArgb(255, 150, 0),
        Color.FromArgb(0, 205, 156),
        Color.FromArgb(255, 134, 208),
        Color.FromArgb(255, 75, 75),
        Color.FromArgb(255, 200, 0)
    };

    public Form2(string selectedCourse)
    {
        course = selectedCourse;
        LoadUserData();
        InitializeForm();
        InitializeSidebar();
        InitializeHeader();
        InitializeMainContent();
        LoadContent();
        this.Resize += Form2_Resize;
    }
    private void Form2_Resize(object sender, EventArgs e)
    {
        UpdateWidgetsWidth();
    }

    private void UpdateWidgetsWidth()
    {
        int availableWidth = flowMain.ClientSize.Width - flowMain.Padding.Left - flowMain.Padding.Right - 20;

        if (panelDailyQuest != null && panelDailyQuest.IsHandleCreated)
        {
            panelDailyQuest.Width = availableWidth;
        }

        if (panelStreak != null && panelStreak.IsHandleCreated)
        {
            panelStreak.Width = availableWidth;
        }

        if (buttonContinue != null && buttonContinue.IsHandleCreated)
        {
            buttonContinue.Width = Math.Min(500, availableWidth);
        }

        if (panelPath != null && panelPath.IsHandleCreated)
        {
            panelPath.Width = availableWidth;

            foreach (Control control in panelPath.Controls)
            {
                if (control is Panel node)
                {
                    control.Width = availableWidth - 40;

                    foreach (Control child in node.Controls)
                    {
                        if (child is Label label && label.Tag?.ToString() == "lessonName")
                        {
                            label.MaximumSize = new Size(availableWidth - 200, 0);
                        }
                    }
                }
            }
        }
    }

    private void LoadUserData()
    {
        //обязательно возьму из базы данных или придумаю суперглобальную переменную, в которой будет хранится значение из предыдущей формы
    }
    private void InitializeForm()
    {
        this.Text = $"LearnCode - Изучайте {course}";
        this.Size = new Size(1100, 750);
        this.BackgroundImage = Image.FromFile("background.png");
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        this.MinimumSize = new Size(900, 600);
    }

    private void InitializeSidebar()
    {
        panelSidebar.BackColor = ColorSnow;
        panelSidebar.Width = 240;
        panelSidebar.Dock = DockStyle.Left;
        panelSidebar.BorderStyle = BorderStyle.None;
        panelSidebar.Paint += (s, e) =>
        {
            e.Graphics.DrawLine(new Pen(ColorSwan, 2), panelSidebar.Width - 2, 0, panelSidebar.Width - 2, panelSidebar.Height);
        };

        PictureBox logo = new PictureBox
        {
            Image = CreateLogo(),
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(140, 50),
            Location = new Point(40, 30),
            Cursor = Cursors.Hand
        };
        logo.Click += (s, e) =>
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        };

        Panel navLearning = CreateNavItem("Обучение", ColorOwl, true, 100);
        Panel navProfile = CreateNavItem("Профиль", ColorPeacock, false, 160);
        Panel navPractice = CreateNavItem("Практика", ColorBeetle, false, 220);
        Panel navProjects = CreateNavItem("Проекты", ColorMacaw, false, 280);

        Panel progressPanel = new Panel
        {
            BackColor = ColorPolar,
            Size = new Size(200, 120),
            Location = new Point(20, 350),
            Padding = new Padding(15)
        };

        Label progressLabel = new Label
        {
            Text = "ПРОГРЕСС",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = ColorHare,
            Location = new Point(0, 0),
            AutoSize = true
        };

        Database2 db = new Database2();
        DataRow[] lessons = db.GetLessons(course);
        int totalLessons = lessons.Length;
        int unlockedLessons = 0;

        foreach (DataRow lesson in lessons)
        {
            if (!(bool)lesson["is_locked"])
            {
                unlockedLessons++;
            }
        }

        int progressPercent = totalLessons > 0 ? (unlockedLessons * 100) / totalLessons : 0;

        Panel progressBarBg = new Panel
        {
            BackColor = ColorSwan,
            Size = new Size(170, 12),
            Location = new Point(0, 40)
        };

        Panel progressBarFill = new Panel
        {
            BackColor = ColorOwl,
            Size = new Size((progressBarBg.Width * progressPercent) / 100, 12),
            Location = new Point(0, 0)
        };

        Label progressText = new Label
        {
            Text = $"{progressPercent}% завершено",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = ColorOwl,
            Location = new Point(0, 65),
            AutoSize = true
        };

        Label progressDetails = new Label
        {
            Text = $"{unlockedLessons} из {totalLessons} уроков",
            Font = new Font("Segoe UI", 9),
            ForeColor = ColorWolf,
            Location = new Point(0, 85),
            AutoSize = true
        };

        progressBarBg.Controls.Add(progressBarFill);
        progressPanel.Controls.Add(progressLabel);
        progressPanel.Controls.Add(progressBarBg);
        progressPanel.Controls.Add(progressText);
        progressPanel.Controls.Add(progressDetails);

        panelSidebar.Controls.Add(logo);
        panelSidebar.Controls.Add(navLearning);
        panelSidebar.Controls.Add(navProfile);
        panelSidebar.Controls.Add(navPractice);
        panelSidebar.Controls.Add(navProjects);
        panelSidebar.Controls.Add(progressPanel);
        this.Controls.Add(panelSidebar);
    }

    private Panel CreateNavItem(string text, Color color, bool isActive, int top)
    {
        Panel panel = new Panel
        {
            Size = new Size(200, 45),
            Location = new Point(20, top),
            Cursor = Cursors.Hand,
            BackColor = isActive ? Color.FromArgb(240, 255, 240) : Color.Transparent
        };

        Label iconLabel = new Label
        {
            Text = text.Substring(0, text.IndexOf(' ') + 1),
            Font = new Font("Segoe UI", 16),
            Location = new Point(10, 10),
            AutoSize = true
        };

        Label textLabel = new Label
        {
            Text = text.Substring(text.IndexOf(' ') + 1),
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            ForeColor = isActive ? color : ColorHare,
            Location = new Point(45, 12),
            AutoSize = true
        };

        if (isActive)
        {
            panel.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(color, 2), 0, 0, panel.Width - 1, panel.Height - 1);
            };
        }

        panel.Controls.Add(iconLabel);
        panel.Controls.Add(textLabel);

        panel.MouseEnter += (s, e) =>
        {
            if (!isActive) panel.BackColor = ColorPolar;
        };

        panel.MouseLeave += (s, e) =>
        {
            if (!isActive) panel.BackColor = Color.Transparent;
        };

        return panel;
    }

    private void InitializeHeader()
    {
        panelHeader.BackColor = ColorSnow;
        panelHeader.Height = 80;
        panelHeader.Dock = DockStyle.Top;
        panelHeader.Padding = new Padding(20, 20, 20, 20);

        Label title = new Label
        {
            Text = course.ToUpper(),
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = ColorEel,
            AutoSize = true,
            Location = new Point(20, 20)
        };

        Panel userPanel = new Panel
        {
            BackColor = ColorPolar,
            Size = new Size(180, 40),
            Location = new Point(panelHeader.Width - 210, 15),
            Padding = new Padding(10, 0, 10, 0)
        };

        Label userLabel = new Label
        {
            Text = userFIO,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = ColorMacaw,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        userPanel.Controls.Add(userLabel);
        panelHeader.Controls.Add(title);
        panelHeader.Controls.Add(userPanel);

        panelHeader.Paint += (s, e) =>
        {
            e.Graphics.DrawLine(new Pen(ColorSwan, 2), 0, panelHeader.Height - 2, panelHeader.Width, panelHeader.Height - 2);
        };

        this.Controls.Add(panelHeader);
    }

    private void InitializeMainContent()
    {
        panelMain.BackColor = ColorPolar;
        panelMain.Dock = DockStyle.Fill;
        panelMain.Padding = new Padding(0, 0, 0, 0);

        flowMain.BackColor = Color.Transparent;
        flowMain.Dock = DockStyle.Fill;
        flowMain.Padding = new Padding(30, 20, 30, 30);
        flowMain.AutoScroll = true;
        flowMain.FlowDirection = FlowDirection.TopDown;
        flowMain.WrapContents = false;
        flowMain.AutoScrollMargin = new Size(0, 20);

        panelMain.Controls.Add(flowMain);
        this.Controls.Add(panelMain);

        this.Controls.SetChildIndex(panelMain, 0);
        this.Controls.SetChildIndex(panelHeader, 1);
        this.Controls.SetChildIndex(panelSidebar, 2);
    }

    private void LoadContent()
    {
        panelDailyQuest = CreateDailyQuestWidget();
        flowMain.Controls.Add(panelDailyQuest);

        panelStreak = CreateStreakWidget();
        flowMain.Controls.Add(panelStreak);

        buttonContinue = new Button
        {
            Text = "НАЧАТЬ УРОК ПРОГРАММИРОВАНИЯ",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = ColorSnow,
            BackColor = ColorOwl,
            Size = new Size(500, 55),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 0, 30)
        };

        buttonContinue.FlatAppearance.BorderSize = 0;
        buttonContinue.FlatAppearance.MouseOverBackColor = ColorTreeFrog;
        buttonContinue.FlatAppearance.MouseDownBackColor = ColorTurtle;
        buttonContinue.Click += (s, e) =>
        {
            Database2 db = new Database2();
            DataRow[] lessons = db.GetLessons(course);

            foreach (DataRow lesson in lessons)
            {
                if (!(bool)lesson["is_locked"])
                {
                    int lessonNumber = (int)lesson["lesson_number"];
                    Form3 form3 = new Form3(lessonNumber, course);
                    form3.Show();
                    this.Hide();
                    return;
                }
            }

            if (lessons.Length > 0)
            {
                Form3 form3 = new Form3(1, course);
                form3.Show();
                this.Hide();
            }
        };

        Panel buttonPanel = new Panel
        {
            Size = new Size(flowMain.ClientSize.Width - 60, 80),
            Padding = new Padding(0, 10, 0, 0)
        };

        buttonPanel.Controls.Add(buttonContinue);
        flowMain.Controls.Add(buttonPanel);

        panelPath = CreateLearningPath();
        flowMain.Controls.Add(panelPath);

        UpdateWidgetsWidth();
    }

    private Panel CreateDailyQuestWidget()
    {
        Panel widget = new Panel
        {
            BackColor = ColorSnow,
            Size = new Size(800, 140),
            Margin = new Padding(0, 0, 0, 20)
        };

        Label title = new Label
        {
            Text = "ЗАДАНИЕ ДЛЯ ПРОГРАММИСТА",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = ColorHare,
            Location = new Point(20, 15),
            AutoSize = true
        };

        Label questText = new Label
        {
            Text = "Решите 3 задачи по программированию",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = ColorEel,
            Location = new Point(20, 45),
            AutoSize = true
        };

        Panel progressContainer = new Panel
        {
            Size = new Size(200, 20),
            Location = new Point(20, 85)
        };

        Panel progressBg = new Panel
        {
            BackColor = ColorSwan,
            Size = new Size(200, 10),
            Location = new Point(0, 5)
        };

        Panel progressFill = new Panel
        {
            BackColor = ColorFox,
            Size = new Size(60, 10),
            Location = new Point(0, 0)
        };

        Label progressLabel = new Label
        {
            Text = "1/3 задач решено",
            Font = new Font("Segoe UI", 10),
            ForeColor = ColorWolf,
            Location = new Point(210, 0),
            AutoSize = true
        };

        progressBg.Controls.Add(progressFill);
        progressContainer.Controls.Add(progressBg);
        progressContainer.Controls.Add(progressLabel);

        widget.Controls.Add(title);
        widget.Controls.Add(questText);
        widget.Controls.Add(progressContainer);

        widget.Paint += (s, e) =>
        {
            e.Graphics.DrawRectangle(new Pen(ColorSwan, 2), 0, 0, widget.Width - 1, widget.Height - 1);
        };

        return widget;
    }

    private Panel CreateStreakWidget()
    {
        Panel widget = new Panel
        {
            BackColor = Color.FromArgb(255, 245, 211),
            Size = new Size(800, 100),
            Margin = new Padding(0, 0, 0, 20)
        };

        Label flameIcon = new Label
        {
            Text = "🔥",
            Font = new Font("Segoe UI", 28),
            Location = new Point(20, 30),
            AutoSize = true
        };

        Label streakText = new Label
        {
            Text = "  ДНЕЙ ПРОГРАММИРОВАНИЯ ПОДРЯД: 7",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = ColorGuineaPig,
            Location = new Point(70, 25),
            AutoSize = true
        };

        Label streakDescription = new Label
        {
            Text = "Пишите код каждый день для лучших результатов!",
            Font = new Font("Segoe UI", 10),
            ForeColor = ColorGuineaPig,
            Location = new Point(70, 55),
            AutoSize = true
        };

        widget.Controls.Add(flameIcon);
        widget.Controls.Add(streakText);
        widget.Controls.Add(streakDescription);

        widget.Paint += (s, e) =>
        {
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(251, 229, 109), 2), 0, 0, widget.Width - 1, widget.Height - 1);
        };

        return widget;
    }

    private Panel CreateLearningPath()
    {
        Database2 db = new Database2();
        DataRow[] lessons = db.GetLessons(course);

        int panelHeight = lessons.Length * 120 + 50;
        Panel widget = new Panel
        {
            BackColor = ColorSnow,
            Size = new Size(800, panelHeight),
            Margin = new Padding(0, 0, 0, 40)
        };

        Label title = new Label
        {
            Text = "ПУТЬ ИЗУЧЕНИЯ ПРОГРАММИРОВАНИЯ",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = ColorHare,
            Location = new Point(0, 0),
            AutoSize = true
        };
        widget.Controls.Add(title);

        for (int i = 0; i < lessons.Length; i++)
        {
            int lessonNumber = (int)lessons[i]["lesson_number"];
            string lessonName = lessons[i]["lesson_name"].ToString();
            bool isLocked = (bool)lessons[i]["is_locked"];

            Panel lessonNode = CreateLessonNode(lessonNumber, lessonName, isLocked, i);
            lessonNode.Location = new Point(0, 50 + i * 120);
            widget.Controls.Add(lessonNode);
        }

        widget.Paint += (s, e) =>
        {
            using (Pen linePen = new Pen(ColorSwan, 3))
            {
                linePen.DashStyle = DashStyle.Dash;
                for (int i = 0; i < lessons.Length - 1; i++)
                {
                    int y1 = 50 + i * 120 + 60;
                    int y2 = 50 + (i + 1) * 120;
                    e.Graphics.DrawLine(linePen, 50, y1, 50, y2);
                }
            }
        };

        widget.Paint += (s, e) =>
        {
            e.Graphics.DrawRectangle(new Pen(ColorSwan, 2), 0, 0, widget.Width - 1, widget.Height - 1);
        };

        return widget;
    }

    private Panel CreateLessonNode(int lessonNumber, string lessonName, bool isLocked, int index)
    {
        Color lessonColor = lessonColors[index % lessonColors.Length];
        Color nodeColor = isLocked ? ColorSwan : lessonColor;
        Color textColor = isLocked ? ColorHare : ColorEel;

        Panel node = new Panel
        {
            Size = new Size(760, 100),
            Location = new Point(20, 0),
            Cursor = isLocked ? Cursors.Default : Cursors.Hand
        };

        Panel circle = new Panel
        {
            BackColor = nodeColor,
            Size = new Size(60, 60),
            Location = new Point(0, 20)
        };
        circle.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, circle.Width, circle.Height);
                circle.Region = new Region(path);
            }

            using (Font font = new Font("Segoe UI", 16, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(isLocked ? ColorHare : ColorSnow))
            {
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(lessonNumber.ToString(), font, brush,
                    new RectangleF(0, 0, circle.Width, circle.Height), format);
            }

            if (isLocked)
            {
                using (Font font = new Font("Segoe UI", 12))
                using (SolidBrush brush = new SolidBrush(ColorHare))
                {
                    e.Graphics.DrawString("🔒", font, brush, 40, 5);
                }
            }
        };

        Label numberLabel = new Label
        {
            Text = $"УРОК {lessonNumber}",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = isLocked ? ColorHare : lessonColor,
            Location = new Point(80, 15),
            AutoSize = true
        };

        Label nameLabel = new Label
        {
            Text = lessonName,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = textColor,
            Location = new Point(80, 40),
            AutoSize = true,
            MaximumSize = new Size(500, 0),
            Tag = "lessonName"
        };

        string icon = GetProgrammingIcon(lessonName);
        Label iconLabel = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI", 20),
            Location = new Point(node.Width - 60, 30),
            AutoSize = true
        };

        node.Controls.Add(circle);
        node.Controls.Add(numberLabel);
        node.Controls.Add(nameLabel);
        node.Controls.Add(iconLabel);

        if (!isLocked)
        {
            EventHandler clickHandler = (s, e) =>
            {
                Form3 form3 = new Form3(lessonNumber, course);
                form3.Show();
                this.Hide();
            };

            node.Click += clickHandler;
            circle.Click += clickHandler;
            numberLabel.Click += clickHandler;
            nameLabel.Click += clickHandler;

            node.MouseEnter += (s, e) =>
            {
                node.BackColor = ColorPolar;
            };

            node.MouseLeave += (s, e) =>
            {
                node.BackColor = Color.Transparent;
            };
        }

        return node;
    }

    private string GetProgrammingIcon(string lessonName)
    {
        string lowerName = lessonName.ToLower();

        if (lowerName.Contains("введение") || lowerName.Contains("основы") || lowerName.Contains("база"))
            return "📚";
        else if (lowerName.Contains("переменн") || lowerName.Contains("variable"))
            return "𝑥";
        else if (lowerName.Contains("тип") || lowerName.Contains("type"))
            return "𝕋";
        else if (lowerName.Contains("оператор") || lowerName.Contains("operator"))
            return "+−×÷";
        else if (lowerName.Contains("услов") || lowerName.Contains("if") || lowerName.Contains("else"))
            return "?";
        else if (lowerName.Contains("цикл") || lowerName.Contains("loop"))
            return "⟳";
        else if (lowerName.Contains("функци") || lowerName.Contains("function"))
            return "ƒ()";
        else if (lowerName.Contains("массив") || lowerName.Contains("array") || lowerName.Contains("list"))
            return "[]";
        else if (lowerName.Contains("объект") || lowerName.Contains("object"))
            return "{}";
        else if (lowerName.Contains("класс") || lowerName.Contains("class"))
            return "𝐂";
        else if (lowerName.Contains("алгоритм") || lowerName.Contains("algorithm"))
            return "⚙";
        else if (lowerName.Contains("ошибк") || lowerName.Contains("error") || lowerName.Contains("exception"))
            return "⚠";
        else if (lowerName.Contains("практик") || lowerName.Contains("practice"))
            return "💻";
        else
            return "";
    }
    private Image CreateLogo()
    {
        Bitmap bmp = new Bitmap(140, 50);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using (Font font = new Font("Segoe UI", 24, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(ColorOwl))
            {
                g.DrawString("", font, brush, new PointF(0, 10));
            }

            using (Font font = new Font("Segoe UI", 20, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(ColorEel))
            {
                g.DrawString("", font, brush, new PointF(25, 12));
            }

            using (Font font = new Font("Segoe UI", 9, FontStyle.Regular))
            using (SolidBrush brush = new SolidBrush(ColorMacaw))
            {
                g.DrawString("", font, brush, new PointF(30, 35));
            }
        }
        return bmp;
    }
}