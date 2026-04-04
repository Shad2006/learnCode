using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;

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
    public class Question
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } 
        public string CorrectAnswer { get; set; }

        public string Options { get; set; }
    }

    public class Database
    {
        private readonly string server = "mytxtwxi.beget.tech";
        private readonly string database = "mytxtwxi_code";
        private readonly string uid = "mytxtwxi_code";
        private readonly string password = "1earnCode";

        private readonly string connectionString;

        public Database()
        {
            connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};CharSet=utf8;SslMode=None;AllowPublicKeyRetrieval=true;";

        }

        private void EnsureTablesExist()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Проверяем наличие таблицы Lessons
                string checkLessons = "SHOW TABLES LIKE 'Lessons';";
                using (var cmd = new MySqlCommand(checkLessons, connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        CreateTables(connection);
                        InsertSampleData(connection);
                    }
                }
            }
        }

        private void CreateTables(MySqlConnection connection)
        {
            string sql = @"
                CREATE TABLE Lessons (
                    lesson_id INT AUTO_INCREMENT PRIMARY KEY,
                    lesson_number INT NOT NULL,
                    lesson_name VARCHAR(255) NOT NULL,
                    course VARCHAR(50) NOT NULL,
                    theory_text TEXT,
                    is_locked BOOLEAN DEFAULT TRUE
                );
                CREATE TABLE Questions (
                    question_id INT AUTO_INCREMENT PRIMARY KEY,
                    lesson_number INT NOT NULL,
                    course VARCHAR(50) NOT NULL,
                    question_text TEXT NOT NULL,
                    question_type VARCHAR(20) NOT NULL,
                    correct_answer VARCHAR(255) NOT NULL
                );";

            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertSampleData(MySqlConnection connection)
        {
            // Вставляем уроки
            AddLesson(1, "Введение в C#", "C#", "Это вводный урок по C#. Здесь вы узнаете основы.", false);
            AddLesson(2, "Переменные и типы данных", "C#", "Урок о переменных, типах int, string и т.д.", true);
            AddLesson(3, "Условные операторы", "C#", "Изучите if, else и switch.", true);

            // Вставляем вопросы
            AddQuestion(1, "C#", "Что такое переменная?", "text", "ячейка памяти");
            AddQuestion(1, "C#", "Какой тип используется для целых чисел?", "choice", "int");
        }
        public List<Lesson> GetLessons(string course)
        {
            var lessons = new List<Lesson>();
            string sql = "SELECT lesson_number, lesson_name, theory_text, is_locked FROM Lessons WHERE course = @course ORDER BY lesson_number";

            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@course", course);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lessons.Add(new Lesson
                        {
                            LessonNumber = reader.GetInt32("lesson_number"),
                            LessonName = reader.GetString("lesson_name"),
                            TheoryText = reader.IsDBNull(reader.GetOrdinal("theory_text")) ? "" : reader.GetString("theory_text"),
                            IsLocked = reader.GetBoolean("is_locked")
                        });
                    }
                }
            }
            return lessons;
        }
        public List<Question> GetQuestions(int lessonNumber, string course)
        {
            var questions = new List<Question>();
            string sql = "SELECT question_text, question_type, correct_answer, options FROM Questions WHERE lesson_number = @lessonNumber AND course = @course";
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@lessonNumber", lessonNumber);
                command.Parameters.AddWithValue("@course", course);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        questions.Add(new Question
                        {
                            QuestionText = reader.GetString("question_text"),
                            QuestionType = reader.GetString("question_type"),
                            CorrectAnswer = reader.GetString("correct_answer"),
                            Options = reader.IsDBNull(reader.GetOrdinal("options")) ? "" : reader.GetString("options")
                        });
                    }
                }
            }
            return questions;
        }
        public void AddLesson(int lessonNumber, string lessonName, string course, string theoryText, bool isLocked)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "INSERT INTO Lessons (lesson_number, lesson_name, course, theory_text, is_locked) VALUES (@n, @name, @c, @theory, @lock)";
                        using (var command = new MySqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@n", lessonNumber);
                            command.Parameters.AddWithValue("@name", lessonName);
                            command.Parameters.AddWithValue("@c", course);
                            command.Parameters.AddWithValue("@theory", theoryText);
                            command.Parameters.AddWithValue("@lock", isLocked);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Добавить вопрос (с транзакцией)
        public void AddQuestion(int lessonNumber, string course, string questionText, string questionType, string correctAnswer)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "INSERT INTO Questions (lesson_number, course, question_text, question_type, correct_answer) VALUES (@ln, @c, @qt, @qtype, @ca)";
                        using (var command = new MySqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ln", lessonNumber);
                            command.Parameters.AddWithValue("@c", course);
                            command.Parameters.AddWithValue("@qt", questionText);
                            command.Parameters.AddWithValue("@qtype", questionType);
                            command.Parameters.AddWithValue("@ca", correctAnswer);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}