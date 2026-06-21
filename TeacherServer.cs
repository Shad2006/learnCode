using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
namespace LearnCode
{
    class TeacherServer
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.db");
        private static HttpListener listener;
        static void TeacherMainWindow(string[] args)
        {
            InitDatabase();
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();
            Console.WriteLine("Сервер запущен на http://*:8080/");
            while (true)
            {
                var context = listener.GetContext();
                ProcessRequest(context);
            }
        }
        public void Start()
        {
            InitDatabase();
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();
            Task.Run(() => ListenAsync());
        }

        public void Stop()
        {
            listener?.Stop();
            listener?.Close();
        }

        private async Task ListenAsync()
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                ProcessRequest(context);
            }
        }
        private static void InitDatabase()
        {
            if (!File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
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
                        lesson INTEGER NOT NULL,
                        score INTEGER DEFAULT 0,
                        answers TEXT,
                        last_code TEXT,
                        completed INTEGER DEFAULT 0,
                        UNIQUE(username, lesson)
                    );
CREATE TABLE IF NOT EXISTS Students (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    FIO TEXT NOT NULL,
    password TEXT NOT NULL
);
                ";
                using (var cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }
        private static void ProcessRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath;
            string method = context.Request.HttpMethod;
            try
            {
                if (path == "/api/lessons" && method == "GET")
                {
                    string course = context.Request.QueryString["course"];
                    string json = GetLessonsJson(course);
                    Respond(context, json);
                }
                else if (path == "/api/questions" && method == "GET")
                {
                    string course = context.Request.QueryString["course"];
                    string lesson = context.Request.QueryString["lesson"];
                    string json = GetQuestionsJson(course, lesson);
                    Respond(context, json);
                }
                else if (path == "/api/auth" && method == "GET")
                {
                    string username = context.Request.QueryString["username"];
                    string password = context.Request.QueryString["password"];
                    bool ok = AuthenticateUser(username, password);
                    string json = $"{{\"success\":{ok.ToString().ToLower()}}}";
                    Respond(context, "{\"success\":true}");
                }
                else if (path == "/api/progress" && method == "POST")
                {
                    string body = new StreamReader(context.Request.InputStream).ReadToEnd();
                    SaveProgress(body);
                    Respond(context, "{\"status\":\"ok\"}");
                }
                else if (path == "/api/progress" && method == "GET")
                {
                    string username = context.Request.QueryString["username"];
                    string course = context.Request.QueryString["course"];
                    string json = GetProgressJson(username, course);
                    Respond(context, json);
                }
                else if (path == "/api/answers" && method == "GET")
                {
                    string id = context.Request.QueryString["id"];
                    int.TryParse(id, out int myint);
                    string json = GetAnswersJson(myint);
                    Respond(context, json);
                }
                else if (path == "/api/completed" && method == "GET")
                {
                    string username = context.Request.QueryString["username"];
                    string course = context.Request.QueryString["course"];
                    string json = GetCompletedLessonsJson(username, course);
                    Respond(context, json);
                }

               
                else
                {
                    Respond(context, "{\"error\":\"not found\"}", 404);
                }
            }
            catch (Exception ex)
            {
                Respond(context, $"{{\"error\":\"{ex.Message}\"}}", 500);
            }
        }
        private static string GetAnswersJson(int id)
        {
            var answers = new Dictionary<int, string>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT answers FROM UserProgress WHERE id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        string answersJson = result.ToString();
                        if (!string.IsNullOrEmpty(answersJson))
                            answers = JsonConvert.DeserializeObject<Dictionary<int, string>>(answersJson);
                    }
                }
            }
            return JsonConvert.SerializeObject(answers);
        }
        private static string GetCompletedLessonsJson(string username, string course)
        {
            var completedLessons = new List<int>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT lesson_number FROM UserProgress WHERE username = @un AND course = @c";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@un", username);
                    cmd.Parameters.AddWithValue("@c", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            completedLessons.Add(reader.GetInt32(0));
                    }
                }
            }
            return JsonConvert.SerializeObject(completedLessons);
        }
        private static bool AuthenticateUser(string username, string password)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Students WHERE FIO = @fio AND password = @kod";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fio", username);
                    cmd.Parameters.AddWithValue("@kod", password);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        private static string GetLessonsJson(string course)
        {
            var lessons = new List<object>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT lesson_number, lesson_name, theory_text, is_locked FROM Lessons WHERE course = @c ORDER BY lesson_number";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@c", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lessons.Add(new
                            {
                                lessonNumber = reader.GetInt32(0),
                                lessonName = reader.GetString(1),
                                theoryText = reader.GetString(2),
                                isLocked = reader.GetInt32(3) == 1
                            });
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(lessons);
        }
        private static string GetQuestionsJson(string course, string lessonNumber)
        {
            var questions = new List<object>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT question_text, question_type, correct_answer, options FROM Questions WHERE course = @c AND lesson_number = @ln";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@c", course);
                    cmd.Parameters.AddWithValue("@ln", int.Parse(lessonNumber));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new
                            {
                                questionText = reader.GetString(0),
                                questionType = reader.GetString(1),
                                correctAnswer = reader.GetString(2),
                                options = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(questions);
        }
        private static void SaveProgress(string jsonBody)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBody);
                string username = data["username"].ToString();
                int lessonNumber = Convert.ToInt32(data["lesson"]);
                string course = data["course"]?.ToString() ?? "";
                int score = Convert.ToInt32(data["score"]);
                string answers = data.ContainsKey("answers") ? data["answers"]?.ToString() : null;
                string lastCode = data.ContainsKey("last_code") ? data["last_code"]?.ToString() : null;
                int completed = Convert.ToInt32(data["completed"]);

                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    string sql = @"INSERT OR REPLACE INTO UserProgress (username, lesson_number, course, completed, score, answers, last_code)
                           VALUES (@un, @ln, @c, @comp, @score, @ans, @lc)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@un", username);
                        cmd.Parameters.AddWithValue("@ln", lessonNumber);
                        cmd.Parameters.AddWithValue("@c", course);
                        cmd.Parameters.AddWithValue("@comp", completed);
                        cmd.Parameters.AddWithValue("@score", score);
                        cmd.Parameters.AddWithValue("@ans", answers ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@lc", lastCode ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveProgress error: {ex.Message}");
            }
        }
        private static string GetProgressJson(string username, string course)
        {
            var progress = new List<object>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT lesson_number, score, completed FROM UserProgress WHERE username = @un AND course = @c";
                Console.WriteLine($"Запрос для username={username}, course={course}");
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@un", username);
                    cmd.Parameters.AddWithValue("@c", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Найдена запись: lesson={reader.GetInt32(0)}, completed={reader.GetInt32(2)}");
                            progress.Add(new
                            {
                                lesson = reader.GetInt32(0),
                                score = reader.GetInt32(1),
                                completed = reader.GetInt32(2) == 1
                            });
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(progress);
        }
        private static void Respond(HttpListenerContext context, string json, int statusCode = 200)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}