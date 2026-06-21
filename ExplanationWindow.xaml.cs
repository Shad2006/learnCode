using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace LearnCodeWPF
{
    public partial class ExplanationWindow : Window
    {
        private ExplanationGenerator generator;

        public ExplanationWindow(string lessonTitle, string lessonText, string language)
        {
            InitializeComponent();
            generator = new ExplanationGenerator();
            generator.StepCompleted += OnStepCompleted;
            generator.ChainCompleted += OnChainCompleted;
            generator.Start(lessonTitle, lessonText, language);
        }

        private void OnStepCompleted(string stepName, string content)
        {
            Dispatcher.Invoke(() =>
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run($"{stepName}\n") { FontWeight = FontWeights.Bold, Foreground = Brushes.DarkBlue });
                paragraph.Inlines.Add(new Run($"{content}\n\n") { Foreground = Brushes.Black });
                rtbSteps.Document.Blocks.Add(paragraph);
                rtbSteps.ScrollToEnd();
                progressBar.Value += 100 / 7.0;
            });
        }

        private void OnChainCompleted(string finalAnswer)
        {
            Dispatcher.Invoke(() =>
            {
                var finalPara = new Paragraph();
                finalPara.Inlines.Add(new Run("РАССУЖДЕНИЕ ЗАВЕРШЕНО\n") { FontWeight = FontWeights.Bold, Foreground = Brushes.Green });
                rtbSteps.Document.Blocks.Clear();
                finalPara.Inlines.Add(new Run(finalAnswer) { Foreground = Brushes.DarkGreen });
                rtbSteps.Document.Blocks.Add(finalPara);
                
                progressBar.IsIndeterminate = false;
                progressBar.Value = 100;
            });
        }
    }
}