using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public static class CodeRunner
{
    public static async Task<string> RunCodeAsync(string code, string language, int timeoutMs = 5000)
    {
        string tempFile = Path.GetTempFileName();
        string output = "";
        try
        {
            if (language == "C#")
            {
                File.WriteAllText(tempFile + ".cs", code);
                var compile = Process.Start(new ProcessStartInfo
                {
                    FileName = "csc.exe",
                    Arguments = $"{tempFile}.cs",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                });
                await compile.WaitForExitAsync();
                if (File.Exists(tempFile + ".exe"))
                {
                    var run = Process.Start(new ProcessStartInfo
                    {
                        FileName = tempFile + ".exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    });
                    await Task.WhenAny(run.WaitForExitAsync(), Task.Delay(timeoutMs));
                    output = await run.StandardOutput.ReadToEndAsync();
                }
                else output = "Ошибка компиляции";
            }
            else if (language == "C++")
            {
                File.WriteAllText(tempFile + ".cpp", code);
                var compile = Process.Start(new ProcessStartInfo
                {
                    FileName = "g++",
                    Arguments = $"{tempFile}.cpp -o {tempFile}.exe",
                    UseShellExecute = false,
                    RedirectStandardError = true
                });
                await compile.WaitForExitAsync();
                if (File.Exists(tempFile + ".exe"))
                {
                    var run = Process.Start(new ProcessStartInfo
                    {
                        FileName = tempFile + ".exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    });
                    await Task.WhenAny(run.WaitForExitAsync(), Task.Delay(timeoutMs));
                    output = await run.StandardOutput.ReadToEndAsync();
                }
                else output = "Ошибка компиляции";
            }
            else if (language == "PHP")
            {
                File.WriteAllText(tempFile + ".php", code);
                var run = Process.Start(new ProcessStartInfo
                {
                    FileName = "php",
                    Arguments = $"{tempFile}.php",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                });
                await Task.WhenAny(run.WaitForExitAsync(), Task.Delay(timeoutMs));
                output = await run.StandardOutput.ReadToEndAsync();
            }
        }
        finally
        {
            try { File.Delete(tempFile + ".cs"); File.Delete(tempFile + ".exe"); File.Delete(tempFile + ".cpp"); File.Delete(tempFile + ".php"); } catch { }
        }
        return output.Trim();
    }
    public static Task WaitForExitAsync(this Process process)
    {
        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;
        process.Exited += (s, e) => tcs.SetResult(null);
        return tcs.Task;
    }
}