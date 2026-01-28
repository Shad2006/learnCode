using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Drawing2D;
public class Form3 : Form
{
    Panel panelMain = new Panel();
    RichTextBox textTheory = new RichTextBox();
    Button buttonStart = new Button();
    Button buttonBack = new Button();
    Button buttonGuide = new Button();
    Panel panelHeader = new Panel();
    int lessonNumber;
    string course;
    public Form3(int number, string crs)
    {
        lessonNumber = number;
        course = crs;
        this.Text = "Урок " + number;
        this.Size = new Size(900, 700);
        this.BackColor = Color.White;
        this.StartPosition = FormStartPosition.CenterScreen;
        panelHeader.BackColor = Color.White;
        panelHeader.Size = new Size(900, 60);
        panelHeader.Dock = DockStyle.Top;
        panelHeader.BorderStyle = BorderStyle.None;
        Label labelHeader = new Label();
        labelHeader.Text = $"Урок {number}";
        labelHeader.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        labelHeader.ForeColor = Color.Black;
        labelHeader.Size = new Size(300, 40);
        labelHeader.Location = new Point(300, 10);
        labelHeader.TextAlign = ContentAlignment.MiddleCenter;
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
            Form2 form2 = new Form2(course);
            form2.Show();
            this.Hide();
        };
        panelHeader.Controls.Add(labelHeader);
        panelHeader.Controls.Add(buttonBack);
        panelMain.Size = new Size(900, 640);
        panelMain.Location = new Point(0, 60);
        panelMain.BackColor = Color.White;
        Label labelTitle = new Label();
        labelTitle.Text = "Введение";
        labelTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
        labelTitle.ForeColor = Color.Black;
        labelTitle.Size = new Size(600, 60);
        labelTitle.Location = new Point(150, 40);
        labelTitle.TextAlign = ContentAlignment.MiddleCenter;
        textTheory.BorderStyle = BorderStyle.None;
        textTheory.Font = new Font("Segoe UI", 14);
        textTheory.ReadOnly = true;
        textTheory.BackColor = Color.White;
        textTheory.Size = new Size(700, 300);
        textTheory.Location = new Point(100, 120);
        textTheory.ScrollBars = RichTextBoxScrollBars.Vertical;
        LoadTheory();
        buttonGuide.Text = "Справочник";
        buttonGuide.Font = new Font("Segoe UI", 12);
        buttonGuide.FlatStyle = FlatStyle.Flat;
        buttonGuide.FlatAppearance.BorderSize = 0;
        buttonGuide.BackColor = Color.FromArgb(240, 240, 240);
        buttonGuide.ForeColor = Color.FromArgb(100, 100, 100);
        buttonGuide.Size = new Size(200, 45);
        buttonGuide.Location = new Point(100, 450);
        buttonGuide.Cursor = Cursors.Hand;
        buttonGuide.Click += (s, e) =>
        {
            MessageBox.Show($"Справочник по {course}\n\nЗдесь будет справочная информация по языку {course}.", "Справочник", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        buttonStart.Text = "начать";
        buttonStart.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        buttonStart.BackColor = Color.FromArgb(88, 179, 104);
        buttonStart.ForeColor = Color.White;
        buttonStart.FlatStyle = FlatStyle.Flat;
        buttonStart.FlatAppearance.BorderSize = 0;
        buttonStart.Size = new Size(200, 50);
        buttonStart.Location = new Point(600, 450);
        buttonStart.Cursor = Cursors.Hand;
        buttonStart.Click += (s, e) =>
        {
            Form4 form4 = new Form4(lessonNumber, course);
            form4.Show();
            this.Hide();
        };
        panelMain.Controls.Add(labelTitle);
        panelMain.Controls.Add(textTheory);
        panelMain.Controls.Add(buttonGuide);
        panelMain.Controls.Add(buttonStart);
        this.Controls.Add(panelHeader);
        this.Controls.Add(panelMain);
    }
    void LoadTheory()
    {
        Database2 db = new Database2();
        DataRow[] lessons = db.GetLessons(course);
        foreach (DataRow lesson in lessons)
        {
            if ((int)lesson["lesson_number"] == lessonNumber)
            {
                textTheory.Text = lesson["theory_text"].ToString();
                break;
            }
        }
    }
}