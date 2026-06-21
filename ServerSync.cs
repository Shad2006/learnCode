using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearnCodeWPF
{
    public static class ServerSync
    {
        private static string serverUrl = "http://127.0.0.1:8080";
        private static readonly HttpClient client = new HttpClient();

        public static void SetServerUrl(string url)
        {
            serverUrl = url;
        }
        public static async Task<bool> AuthAsync(string username, string password)
        {
            try
            {
                string url = $"{serverUrl}/api/auth?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";
                string response = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(response);
                return result != null && result.ContainsKey("success") && result["success"];
            }
            catch { return false; }
        }
        public static async Task<List<Lesson>> FetchLessonsAsync(string course)
        {
            try
            {
                string url = $"{serverUrl}/api/lessons?course={Uri.EscapeDataString(course)}";
                string json = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<Lesson>>(json);
            }
            catch { return null; }
        }
        public static async Task<bool> SendProgressAsync(string username, int lesson, string course, int score, string answers, bool completed)
        {
            try
            {
                var data = new { username, lesson, course, score, answers, completed };
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{serverUrl}/api/progress", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        public static async Task<List<int>> FetchCompletedLessonsAsync(string username, string course)
        {
            try
            {
                string url = $"{serverUrl}/api/completed?username={Uri.EscapeDataString(username)}&course={Uri.EscapeDataString(course)}";
                string json = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<int>>(json);
            }
            catch { return null; }
        }
        public static async Task<Dictionary<int, (int score, bool completed)>> FetchProgressAsync(string username, string course)
        {
            try
            {
                string url = $"{serverUrl}/api/progress?username={Uri.EscapeDataString(username)}&course={Uri.EscapeDataString(course)}";
                string json = await client.GetStringAsync(url);
                var list = JsonConvert.DeserializeObject<List<ProgressItem>>(json);
                var dict = new Dictionary<int, (int, bool)>();
                foreach (var item in list)
                {
                    dict[item.lesson] = (item.score, item.completed == 1);
                }
                return dict;
            }
            catch { return null; }
        }
        public static async Task<List<Question>> FetchQuestionsAsync(string course, int lessonNumber)
        {
            try
            {
                string url = $"{serverUrl}/api/questions?course={Uri.EscapeDataString(course)}&lesson={lessonNumber}";
                string json = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<Question>>(json);
            }
            catch
            {
                return null;
            }
        }
        public static async Task<Dictionary<int, string>> FetchAnswersAsync(int id)
        {
            try
            {
                string url = $"{serverUrl}/api/answers?id={id}";
                string json = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
            }
            catch { return null; }
        }
        private class ProgressItem
        {
            public int lesson { get; set; }
            public int score { get; set; }
            public string answers { get; set; }
            public string lastCode { get; set; }
            public int completed { get; set; }
        }
    }
}