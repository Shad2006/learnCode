using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace LearnCodeWPF
{
    public static class CodeRunner
    {
        public static async Task<string> RunCodeAsync(string code, string language)
        {
            if (language == "C#")
            {
                return await Task.Run(() => RunCSharpWithCodeDom(code));
            }
            else if (language == "C++")
            {
                return await RunCppAsync(code);
            }
            else if (language == "PHP")
            {
                return await RunPhpAsync(code);
            }
            {
                return $"Язык '{language}' не поддерживается для выполнения кода.";
            }
        }
        private static string RunCSharpWithCodeDom(string userCode)
        {
            string wrapper = @"
using System;
using System.IO;

public class UserSolution
{
    public string Execute()
    {
        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);
            try
            {
                // ---------- Код пользователя ----------
                // USER_CODE_HERE
                // --------------------------------------
                return sw.ToString();
            }
            catch (Exception ex)
            {
                return ""Ошибка выполнения: "" + ex.Message;
            }
            finally
            {
                var stdout = new StreamWriter(Console.OpenStandardOutput());
                stdout.AutoFlush = true;
                Console.SetOut(stdout);
            }
        }
    }
}";
            string fullCode = wrapper.Replace("// USER_CODE_HERE", userCode);

            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");

            using (var provider = new CSharpCodeProvider())
            {
                CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, fullCode);
                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Ошибка компиляции:");
                    foreach (CompilerError err in results.Errors)
                    {
                        sb.AppendLine($"  Строка {err.Line}: {err.ErrorText}");
                    }
                    return sb.ToString();
                }

                var assembly = results.CompiledAssembly;
                var type = assembly.GetType("UserSolution");
                var instance = Activator.CreateInstance(type);
                var method = type.GetMethod("Execute");
                string output = method.Invoke(instance, null) as string;
                return output?.Trim() ?? "";
            }
        }
        private static async Task<string> RunCppAsync(string code)
        {
            string tempFile = Path.GetTempFileName();
            string output = "";
            try
            {
                File.WriteAllText(tempFile + ".cpp", code);
                string gccPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gcc", "bin", "g++.exe");
                if (!File.Exists(gccPath))
                {
                    return "Компилятор C++ не найден по пути: " + gccPath;
                }

                var compile = Process.Start(new ProcessStartInfo
                {
                    FileName = gccPath,
                    Arguments = $"{tempFile}.cpp -o {tempFile}.exe",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                });
                await WaitForExitAsync(compile);
                if (File.Exists(tempFile + ".exe"))
                {
                    var run = Process.Start(new ProcessStartInfo
                    {
                        FileName = tempFile + ".exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    });
                    await WaitForExitAsync(run);
                    output = await run.StandardOutput.ReadToEndAsync();
                }
                else
                {
                    output = "Ошибка компиляции C++:\n" + await compile.StandardError.ReadToEndAsync();
                }
            }
            finally
            {
                try { File.Delete(tempFile + ".cpp"); } catch { }
                try { File.Delete(tempFile + ".exe"); } catch { }
            }
            return output.Trim();
        }
        private static async Task<string> RunPhpAsync(string code)
        {
            string tempFile = Path.GetTempFileName();
            string output = "";
            try
            {
                File.WriteAllText(tempFile + ".php", code);
                var run = Process.Start(new ProcessStartInfo
                {
                    FileName = "php",
                    Arguments = $"{tempFile}.php",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                });
                await WaitForExitAsync(run);
                output = await run.StandardOutput.ReadToEndAsync();
            }
            finally
            {
                try { File.Delete(tempFile + ".php"); } catch { }
            }
            return output.Trim();
        }

        private static Task WaitForExitAsync(Process process)
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => tcs.SetResult(null);
            return tcs.Task;
        }
    }
}