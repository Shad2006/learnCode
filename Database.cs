using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace LearnCodeWPF
{
    // Модель урока
    public class Lesson
    {
        public int LessonNumber { get; set; }
        public string LessonName { get; set; }
        public string TheoryText { get; set; }
        public bool IsLocked { get; set; }
    }

    // Модель вопроса
    public class Question
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }   // "text", "choice", "code"
        public string CorrectAnswer { get; set; }
        public string Options { get; set; }        // для choice – варианты через '|'
    }

    public class Database
    {
        // Абсолютный путь к файлу БД – всегда в папке с EXE
        private readonly string connectionString;

        public Database()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "learning.db");
            connectionString = $"Data Source={dbPath};Version=3;Journal Mode=Wal;";

            // Если файла нет – создаём и инициализируем таблицы
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                InitializeTables();
                InsertSampleData(); // тестовые данные, можно закомментировать
            }
        }

        // Создание таблиц, если их нет
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

        // Вставить начальные данные (если нужно)
        private void InsertSampleData()
        {
            // Проверим, есть ли уже уроки
            var existing = GetLessons("C#");
            if (existing.Count > 0) return;

            // C# уроки
            AddLesson(1, "Введение в C#", "C#", "Это вводный урок по C#. Здесь вы узнаете основы.", false);
            AddLesson(2, "Переменные и типы данных", "C#", "Урок о переменных, типах int, string и т.д.", true);
            AddLesson(3, "Условные операторы", "C#", "Изучите if, else и switch.", true);

            // Вопросы для C# (урок 1)
            AddQuestion(1, "C#", "Что такое переменная?", "text", "ячейка памяти", null);
            AddQuestion(1, "C#", "Какой тип используется для целых чисел?", "choice", "int", "int|string|bool|double");
        }

        // ========== РАБОТА С УРОКАМИ ==========
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

        // ========== РАБОТА С ВОПРОСАМИ ==========
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

        // ========== ПРОГРЕСС ПОЛЬЗОВАТЕЛЯ ==========
        public void MarkLessonCompleted(int lessonNumber, string course, int score)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                // user_id = 1 (один пользователь, без регистрации)
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

        // Получить общий прогресс по курсу (количество завершённых уроков)
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

        // Получить общее количество уроков в курсе
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
    }
}