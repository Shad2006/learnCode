using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
namespace LearnCodeWPF
{
    public class Lesson
    {
        public int LessonNumber { get; set; }
        public string LessonName { get; set; }
        public string TheoryText { get; set; }
        public bool IsLocked { get; set; }
        public string Course { get; set; }
    }
    public class Question
    {
        public int LessonNumber { get; set; }
        public string Course { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string CorrectAnswer { get; set; }
        public string Options { get; set; }
    }
    public class Database
    {
        private string connectionString;
        public Database()
        {
            string dbName = "learning.db";
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName);
            connectionString = $"Data Source={dbPath};Version=3;";
        }
        private void CreateDatabase(string dbPath)
        {
            SQLiteConnection.CreateFile(dbPath);
            InitializeTables();
        }
        private void InitializeTables()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Lessons (
                        lesson_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        lesson_number INTEGER NOT NULL,
                        lesson_name TEXT NOT NULL,
                        course TEXT NOT NULL,
                        theory_text TEXT,
                        is_locked INTEGER DEFAULT 1
                    );
                    CREATE TABLE IF NOT EXISTS Questions (
                        question_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        lesson_number INTEGER NOT NULL,
                        course TEXT NOT NULL,
                        question_text TEXT NOT NULL,
                        question_type TEXT NOT NULL,
                        correct_answer TEXT NOT NULL,
                        options TEXT
                    );
                    CREATE TABLE IF NOT EXISTS UserProgress (
                        user_id INTEGER DEFAULT 1,
                        lesson_number INTEGER NOT NULL,
                        course TEXT NOT NULL,
                        completed INTEGER DEFAULT 0,
                        score INTEGER DEFAULT 0,
                        PRIMARY KEY (user_id, lesson_number, course)
                    );
                ";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Lesson> GetLessons(string course)
        {
            var lessons = new List<Lesson>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT lesson_number, lesson_name, theory_text, is_locked FROM Lessons WHERE course = @course ORDER BY lesson_number";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@course", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lessons.Add(new Lesson
                            {
                                LessonNumber = reader.GetInt32(0),
                                LessonName = reader.GetString(1),
                                TheoryText = reader.GetString(2),
                                IsLocked = reader.GetInt32(3) == 1
                            });
                        }
                    }
                }
            }
            return lessons;
        }
        public bool Auth(string login, string password)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Students WHERE FIO = @login AND password = @password";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
        }
        public void AddLesson(int lessonNumber, string lessonName, string course, string theoryText, bool isLocked)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Lessons (lesson_number, lesson_name, course, theory_text, is_locked) VALUES (@n, @name, @c, @theory, @lock)";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@n", lessonNumber);
                    cmd.Parameters.AddWithValue("@name", lessonName);
                    cmd.Parameters.AddWithValue("@c", course);
                    cmd.Parameters.AddWithValue("@theory", theoryText);
                    cmd.Parameters.AddWithValue("@lock", isLocked ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Question> GetQuestions(int lessonNumber, string course)
        {
            var questions = new List<Question>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT question_text, question_type, correct_answer, options FROM Questions WHERE lesson_number = @ln AND course = @c";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    cmd.Parameters.AddWithValue("@c", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new Question
                            {
                                QuestionText = reader.GetString(0),
                                QuestionType = reader.GetString(1),
                                CorrectAnswer = reader.GetString(2),
                                Options = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return questions;
        }

        public void AddQuestion(int lessonNumber, string course, string questionText, string questionType, string correctAnswer, string options = null)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Questions (lesson_number, course, question_text, question_type, correct_answer, options) VALUES (@ln, @c, @qt, @qtype, @ca, @opt)";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    cmd.Parameters.AddWithValue("@c", course);
                    cmd.Parameters.AddWithValue("@qt", questionText);
                    cmd.Parameters.AddWithValue("@qtype", questionType);
                    cmd.Parameters.AddWithValue("@ca", correctAnswer);
                    cmd.Parameters.AddWithValue("@opt", options ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MarkLessonCompleted(int lessonNumber, string course, int score)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"INSERT OR REPLACE INTO UserProgress (user_id, lesson_number, course, completed, score)
                               VALUES (1, @ln, @c, 1, @score)";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    cmd.Parameters.AddWithValue("@c", course);
                    cmd.Parameters.AddWithValue("@score", score);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool IsLessonCompleted(int lessonNumber, string course)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT completed FROM UserProgress WHERE user_id = 1 AND lesson_number = @ln AND course = @c";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    cmd.Parameters.AddWithValue("@c", course);
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
        }
        public void NextLesson(string course, int it)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE lessons SET is_locked=0 WHERE course = @c AND lesson_number=@ln";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ln", it + 1);
                    cmd.Parameters.AddWithValue("@c", course);
                    cmd.ExecuteScalar();
                }
            }
        }
        public int GetCompletedLessonsCount(string course)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM UserProgress WHERE user_id = 1 AND course = @c AND completed = 1";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@c", course);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int GetTotalLessonsCount(string course)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Lessons WHERE course = @c";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@c", course);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public List<Question> GetAllQuestions()
        {
            var questions = new List<Question>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT lesson_number, course, question_text, question_type, correct_answer, options FROM Questions";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        questions.Add(new Question
                        {
                            LessonNumber = reader.GetInt32(0),
                            Course = reader.GetString(1),
                            QuestionText = reader.GetString(2),
                            QuestionType = reader.GetString(3),
                            CorrectAnswer = reader.GetString(4),
                            Options = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }
                }
            }
            return questions;
        }
        public void UpdateLesson(Lesson lesson, string course)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Lessons SET lesson_name=@name, theory_text=@theory, is_locked=@lock WHERE lesson_number=@num AND course=@course";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", lesson.LessonName);
                    cmd.Parameters.AddWithValue("@theory", lesson.TheoryText);
                    cmd.Parameters.AddWithValue("@lock", lesson.IsLocked ? 1 : 0);
                    cmd.Parameters.AddWithValue("@num", lesson.LessonNumber);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteLesson(int lessonNumber, string course)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Lessons WHERE lesson_number=@num AND course=@course";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@num", lessonNumber);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddLessonComplete(Lesson lesson, string course)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Lessons (lesson_number, lesson_name, course, theory_text, is_locked) VALUES (@num, @name, @course, @theory, @lock)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@num", lesson.LessonNumber);
                    cmd.Parameters.AddWithValue("@name", lesson.LessonName);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@theory", lesson.TheoryText);
                    cmd.Parameters.AddWithValue("@lock", lesson.IsLocked ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateQuestion(int lessonNumber, string course, string oldQuestionText, string newQuestionText, string type, string correctAnswer, string options)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Questions SET question_text=@newText, question_type=@type, correct_answer=@correct, options=@options 
                       WHERE lesson_number=@ln AND course=@course AND question_text=@oldText";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@newText", newQuestionText);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@correct", correctAnswer);
                    cmd.Parameters.AddWithValue("@options", options ?? "");
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@oldText", oldQuestionText);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteQuestion(int lessonNumber, string course, string questionText)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Questions WHERE lesson_number=@ln AND course=@course AND question_text=@text";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@text", questionText);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddStudent(string name, string password)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Students  (FIO,password) VALUES (@fio, @password)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fio", name);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddQuestionComplete(Question question, string course)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Questions (lesson_number, course, question_text, question_type, correct_answer, options) 
                       VALUES (@ln, @course, @text, @type, @correct, @options)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ln", question.LessonNumber);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@text", question.QuestionText);
                    cmd.Parameters.AddWithValue("@type", question.QuestionType);
                    cmd.Parameters.AddWithValue("@correct", question.CorrectAnswer);
                    cmd.Parameters.AddWithValue("@options", question.Options ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}