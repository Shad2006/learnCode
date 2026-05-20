using System.Collections.Generic;
using System.Windows;
using LearnCode;
namespace LearnCodeWPF
{
    public partial class TeacherMainWindow : Window
    {
        private Database db;
        public TeacherMainWindow()
        {
            InitializeComponent();
            db = new Database();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var list = new List<StudentStat>
            {
                new StudentStat { StudentName = "Алексей", Course = "C#", CompletedLessons = 3, AverageScore = 85 },
                new StudentStat { StudentName = "Мария", Course = "C++", CompletedLessons = 2, AverageScore = 70 }
            };
            dgStudents.ItemsSource = list;
        }
        private void BtnEditContent_Click(object sender, RoutedEventArgs e)
        {
            TeacherEditWindow editWin = new TeacherEditWindow();
            editWin.ShowDialog();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {

            RegWindow wind = new RegWindow();
            wind.ShowDialog();
        }
    }

    public class StudentStat
    {
        public string StudentName { get; set; }
        public string Course { get; set; }
        public int CompletedLessons { get; set; }
        public int AverageScore { get; set; }
    }
}