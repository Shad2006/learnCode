using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Text.RegularExpressions;
namespace LearnCodeWPF
{
    public class ExplanationGenerator
    {
        public event Action<string, string> StepCompleted; 
        public event Action<string> ChainCompleted;   
        private string lessonText;
        private string lessonTitle;
        private string language;
        private List<string> memoryBank;
        private List<string> generatedSteps;
        public string GetPlainTextFromHtml(string html)
        {
            string clean = Regex.Replace(html, @"<[^>]*>", ""); 
            clean = Regex.Replace(clean, @"&nbsp;|&lt;|&gt;|&amp;", m =>
            m.Value == "&nbsp;" ? " " : (m.Value == "&lt;" ? "<" : (m.Value == "&gt;" ? ">" : "&")));
            clean = Regex.Replace(clean, @"\s+", " ").Trim();
            return clean;
        }
        public void Start(string title, string text, string lang = "C#")
        {
            lessonTitle = title;
            lessonText = text;
            lessonText = GetPlainTextFromHtml(lessonText);
            language = lang;
            memoryBank = new List<string>
            {
                "Переменные хранят данные.",
                "Функции выполняют задачи.",
                "Циклы повторяют код.",
                "Условия управляют потоком.",
                "Массивы хранят наборы элементов."
            };
            generatedSteps = new List<string>();
            ProcessNextStep(1);
        }

        private async void ProcessNextStep(int step)
        {
            switch (step)
            {
                case 1: await Step_Understand(); break;
                case 2: await Step_Decompose(); break;
                case 3: await Step_Recall(); break;
                case 4: await Step_Analyze(); break;
                case 5: await Step_Draft(); break;
                case 6: await Step_Refine(); break;
                case 7: await Step_Finalize(); break;
                default:
                    ChainCompleted?.Invoke(string.Join("\n\n", generatedSteps));
                    return;
            }
            ProcessNextStep(step + 1);
        }

        private async Task Step_Understand()
        {
            string content = $"Тема: {lessonTitle}\nЯзык: {language}\nИсходный текст:\n{lessonText.Substring(0, Math.Min(200, lessonText.Length))}...";
            StepCompleted?.Invoke("Осознание", content);
            await Task.Delay(300);
        }

        private async Task Step_Decompose()
        {
            StepCompleted?.Invoke("Декомпозиция", "Разбиваю объяснение на логические блоки: определение, примеры, ключевые моменты.");
            await Task.Delay(300);
        }

        private async Task Step_Recall()
        {
            var keywords = lessonText.Split(' ', '.', ',', ';', '!', '?')
                                     .Where(w => w.Length > 5)
                                     .Select(w => w.ToLower())
                                     .Distinct();
            var found = memoryBank.Where(m => keywords.Any(k => m.ToLower().Contains(k))).ToList();
            string content = "Найдено в памяти:\n" + (found.Any() ? string.Join("\n", found) : "Нет прямых совпадений, использую общие принципы.");
            StepCompleted?.Invoke("Воспоминания", content);
            await Task.Delay(300);
        }

        private async Task Step_Analyze()
        {
            StepCompleted?.Invoke("Анализ", "Выделяю ключевые понятия и связи между ними.");
            await Task.Delay(300);
        }

        private async Task Step_Draft()
        {
            string draft = "Черновик объяснения:\n\n";
            draft += $"**{lessonTitle}** — это раздел, где изучается:\n";
            draft += $"- {SimplifyText(lessonText)}\n";
            draft += $"- Пример: {GetExampleForLanguage(language)}";
            StepCompleted?.Invoke("Черновик", draft);
            generatedSteps.Add(draft);
            await Task.Delay(300);
        }

        private async Task Step_Refine()
        {
            StepCompleted?.Invoke("Улучшение", "Добавляю аналогии из реальной жизни и убираю сложные термины.");
            await Task.Delay(300);
        }

        private async Task Step_Finalize()
        {
            string final = "**Итоговое простое объяснение:**\n\n" + SimplifyText(lessonText);
            StepCompleted?.Invoke("Финализация", final);
            generatedSteps.Add(final);
            await Task.Delay(300);
        }
        private string SimplifyText(string original)
        {
            var simplifications = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "переменная", "ячейка для хранения данных (как коробка с именем)" },
                { "тип данных", "вид информации (число, текст, да/нет)" },
                { "цикл", "повторение действий несколько раз" },
                { "условный оператор", "проверка условия и выбор пути" },
                { "массив", "список элементов одного типа" },
                { "функция", "именованный блок кода, который можно вызывать" }
            };
            string simplified = original;
            foreach (var kv in simplifications)
            {
                simplified = simplified.Replace(kv.Key, kv.Value);
            }
            simplified += $"\n\nПроще говоря: {GetSimpleAnalogy(lessonTitle)}";
            return simplified;
        }

        private string GetExampleForLanguage(string lang)
        {
            if (lang == "C#") return "int age = 25; // объявление переменной";
            if (lang == "C++") return "int age = 25; // то же самое, но без управляемой памяти";
            if (lang == "PHP") return "$age = 25; // переменная без указания типа";
            return "код на вашем языке";
        }

        private string GetSimpleAnalogy(string title)
        {
            if (title.Contains("Переменные")) return "переменная — это коробка с наклейкой, куда можно положить число или текст.";
            if (title.Contains("Циклы")) return "цикл — как многократное прослушивание любимой песни, пока не надоест.";
            if (title.Contains("Условные")) return "условие — как если на улице дождь, то беру зонт, иначе — нет.";
            if (title.Contains("Массивы")) return "массив — это как список покупок: все элементы в одном месте.";
            return "это основа программирования, которую можно сравнить с рецептом приготовления блюда: шаг за шагом.";
        }
    }
}