using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
namespace LearnCode
{
    public class ProgressDisplay
    {
        public string StudentName { get; set; }
        public string Course { get; set; }
        public string LessonName { get; set; }
        public int LessonNumber { get; set; }
        public int Score { get; set; }
        public string Status { get; set; }
        public int Id { get; set; }
    }
    public class Lesson
    {
        public int id { get; set; }
        public int LessonNumber { get; set; }
        public string LessonName { get; set; }
        public string TheoryText { get; set; }
        public bool IsLocked { get; set; }
        public string Course { get; set; }
    }
    public class Question
    {
        public int id { get; set; }
        public int LessonNumber { get; set; }
        public string Course { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string CorrectAnswer { get; set; }
        public string Options { get; set; }
    }
    public class students
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Password { get; set; }
    }
    public class ServerDatabase
    {
        private string connectionString;
        public ServerDatabase()
        {
            string dbName = "server.db";
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.db");
        connectionString = $"Data Source={dbPath};Version=3;";
            if (!File.Exists(dbPath)) CreateDatabase(dbPath);
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
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL,
    lesson_number INTEGER NOT NULL,
    course TEXT NOT NULL,
    completed INTEGER DEFAULT 0,
    score INTEGER DEFAULT 0,
    answers TEXT,
    last_code TEXT,
    UNIQUE(username, lesson_number, course)
);

CREATE TABLE IF NOT EXISTS Students (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    FIO TEXT NOT NULL,
    password TEXT NOT NULL
);
                ";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public (string username, string course, int lessonNumber) GetProgressDetails(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT username, course, lesson_number FROM UserProgress WHERE id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (reader.GetString(0), reader.GetString(1), reader.GetInt32(2));
                        }
                    }
                }
            }
            return (null, null, 0);
        }
        public void DeleteStudent(string fio)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sqlStudent = "DELETE FROM Students WHERE FIO = @fio";
                using (var cmd = new SQLiteCommand(sqlStudent, conn))
                {
                    cmd.Parameters.AddWithValue("@fio", fio);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ChangeStudentPassword(string fio, string newPassword)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Students SET password = @pwd WHERE FIO = @fio";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pwd", newPassword);
                    cmd.Parameters.AddWithValue("@fio", fio);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Dictionary<int, (int score, bool completed)> GetUserProgress(string username, string course)
        {
            var dict = new Dictionary<int, (int, bool)>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT lesson_number, score, completed FROM UserProgress WHERE username = @un AND course = @c";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@un", username);
                    cmd.Parameters.AddWithValue("@c", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int ln = reader.GetInt32(0);
                            int sc = reader.GetInt32(1);
                            bool comp = reader.GetInt32(2) == 1;
                            dict[ln] = (sc, comp);
                        }
                    }
                }
            }
            return dict;
        }
        public List<Lesson> GetLessons(string course)
        {
            var lessons = new List<Lesson>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT lesson_id, lesson_number, lesson_name, theory_text, is_locked FROM Lessons WHERE course = @course ORDER BY lesson_number";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@course", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lessons.Add(new Lesson
                            {
                                id = reader.GetInt32(0),
                                LessonNumber = reader.GetInt32(1),
                                LessonName = reader.GetString(2),
                                TheoryText = reader.GetString(3),
                                IsLocked = reader.GetInt32(4) == 1
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
        public List<ProgressDisplay> GetAllProgressRecords()
        {
            var list = new List<ProgressDisplay>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"
            SELECT 
                s.FIO,

                up.course,
                l.lesson_name,
up.lesson_number,
                up.score,
                up.completed,
up.id
            FROM UserProgress up
            JOIN Students s ON up.username = s.FIO
            JOIN Lessons l ON up.course = l.course AND up.lesson_number = l.lesson_number
            ORDER BY s.FIO, up.course, l.lesson_number";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProgressDisplay
                        {
                            StudentName = reader.GetString(0),
                            Course = reader.GetString(1),
                            LessonName = reader.GetString(2),
                            LessonNumber = reader.GetInt32(3),
                            Score = reader.GetInt32(4),
                            Status = reader.GetInt32(5) == 1 ? "Пройден" : "Не пройден",
                            Id = reader.GetInt32(6)
                        });
                    }
                }
            }
            return list;
        }
        public List<Question> GetQuestions(int lessonNumber, string course)
        {
            var questions = new List<Question>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT question_id, question_text, question_type, correct_answer, options FROM Questions WHERE lesson_number = @ln AND course = @c";
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
                                id = Convert.ToInt32(reader["question_id"]),
                                QuestionText = reader.GetString(reader.GetOrdinal("question_text")),
                                QuestionType = reader.GetString(reader.GetOrdinal("question_type")),
                                CorrectAnswer = reader.GetString(reader.GetOrdinal("correct_answer")),
                                Options = reader.IsDBNull(reader.GetOrdinal("options")) ? null : reader.GetString(reader.GetOrdinal("options"))
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
        public string getlastcode(string username, string course, int lessonnumber)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT last_code FROM UserProgress WHERE username = @username AND course=@course AND lesson_number=@ln";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@ln", lessonnumber);
                    object result = cmd.ExecuteScalar();
                    return result == null ? null : result.ToString();
                }
            }
        }
        public void insertLastCode(string username, string course, int lessonNumber, string code)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string updateSql = "UPDATE UserProgress SET last_code = @code WHERE username = @username AND course = @course AND lesson_number = @ln";
                using (var cmd = new SQLiteCommand(updateSql, conn))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        string insertSql = "INSERT INTO UserProgress (username, course, lesson_number, last_code) VALUES (@username, @course, @ln, @code)";
                        using (var insertCmd = new SQLiteCommand(insertSql, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@username", username);
                            insertCmd.Parameters.AddWithValue("@course", course);
                            insertCmd.Parameters.AddWithValue("@ln", lessonNumber);
                            insertCmd.Parameters.AddWithValue("@code", code);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        public void MarkLessonCompleted(string username, int lessonNumber, string course, int score, string answers = null, string lastCode = null)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT OR REPLACE INTO UserProgress (username, lesson_number, course, completed, score, answers, last_code)
                       VALUES (@un, @ln, @c, 1, @score, @ans, @code)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@un", username);
                    cmd.Parameters.AddWithValue("@ln", lessonNumber);
                    cmd.Parameters.AddWithValue("@c", course);
                    cmd.Parameters.AddWithValue("@score", score);
                    cmd.Parameters.AddWithValue("@ans", answers ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@code", lastCode ?? (object)DBNull.Value);
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
        public List<students> GetAllStudents()
        {
            var list = new List<students>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, FIO, password FROM Students";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new students
                        {
                            Id = reader.GetInt32(0),
                            FIO = reader.GetString(1),
                            Password = reader.GetString(2)
                        });
                    }
                }
            }
            return list;
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
        public void DeleteLesson(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Lessons WHERE lesson_id=@id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
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
        public void DeleteQuestion(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Questions WHERE question_id=@id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
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