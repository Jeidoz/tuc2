using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using tuc2.DataTypes;
using Tuc2DDL;
using Tuc2DDL.Entities;

namespace tuc2.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for TestingWnd.xaml
    /// </summary>
    public partial class TestingWnd : Window
    {
        public class TestingAction
        {
            public string Action { get; set; }

            public TestingAction()
            {

            }
            public TestingAction(string action)
            {
                Action = action;
            }
        }

        private readonly DbContext db;
        private readonly Exercise task;
        private readonly List<Test> tests;
        private readonly FileInfo codeFileInfo;
        private bool isCompiled;
        private bool isTestRunPassed;
        private int testNumber;
        private int passedTests;
        private int failedTests;
        private int processTimeLimit;

        public ObservableCollection<TestingAction> ActionList { get; set; }

        public TestingWnd(int taskId, string codeFile)
        {
            processTimeLimit = (int)TimeSpan.FromSeconds(3).TotalMilliseconds;
            db = WpfHelper.Database;
            task = db.GetExercise(taskId);
            tests = task.Tests;
            codeFileInfo = new FileInfo(GetDistFilePath(codeFile));

            InitializeActionList();

            DataContext = this;
            InitializeComponent();
        }

        private string GetDistFilePath(string fileName)
        {
            var currentDir = Directory.GetCurrentDirectory();
            return Path.Combine(currentDir, "Codes", fileName);
        }
        private void InitializeActionList()
        {
            ActionList = new ObservableCollection<TestingAction>()
            {
                new TestingAction("Ініціалізація змінних..."),
                new TestingAction("Зчитування контрольних тестів..."),
                new TestingAction($"Підготовка до тестування завдання із назвою \"{task.Name}\" завершена."),
                new TestingAction("Компіляція коду...")
            };
        }

        private TestingAction AddNewAction(string action)
        {
            var newAction = new TestingAction(action);
            ActionList.Add(newAction);
            return newAction;
        }

        private async Task<CompilationResult> Compile()
        {
            string extension = codeFileInfo.Extension;
            string languageCompiler = string.Empty;
            switch (extension)
            {
                case ".cpp":
                case ".c":
                    languageCompiler = "g++";
                    break;
                case ".pas":
                    languageCompiler = "fpc";
                    break;
                case ".py":
                    return new CompilationResult
                    {
                        IsCompiled = true,
                        Errors = "Код написаний на Python не потребує компіляції!"
                    };
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = languageCompiler,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string fileName = codeFileInfo.FullName;
            if (extension == ".cpp" || extension == ".c")
            {
                var exeName = fileName.Replace(codeFileInfo.Extension, ".exe");
                startInfo.Arguments = $" \"{fileName}\" -o \"{exeName}\"";
            }
            else
            {
                startInfo.Arguments = $"\"{fileName}\"";
            }

            var process = Process.Start(startInfo);
            var errors = await process.StandardError.ReadToEndAsync();
            var output = await process.StandardOutput.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(errors))
            {
                errors = output;
            }

            if (errors.Contains("compiled"))
            {
                errors = string.Empty;
            }

            process.WaitForExit();
            return new CompilationResult(errors);
        }
        private async Task ProcessCompilation()
        {
            var compilationResult = await Compile();
            if (compilationResult.IsCompiled)
            {
                progressBarStatus.Value = 10;
                AddNewAction("Компіляція завершена.");
                isCompiled = true;
            }
            else
            {
                progressBarStatus.Foreground = Brushes.MediumVioletRed;
                AddNewAction("Помилка компіляції!");
                AddNewAction($"Дані про помилку:\n{compilationResult.Errors}");
                isCompiled = false;
            }
        }
        private Task<RuntimeResult> RunProcessAsync(ProcessStartInfo startInfo, string input)
        {
            return Task.Factory.StartNew(() =>
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.StandardInput.WriteLine(input);
                    var processResult = true;
                    var firstEntry = true;
                    do
                    {
                        process.Refresh();
                        if (firstEntry)
                        {
                            firstEntry = false;
                            continue;
                        }
                        if (!process.HasExited)
                        {
                            processResult = false;
                            break;
                        }
                    } while (!process.WaitForExit(processTimeLimit));

                    var errors = string.Empty;
                    if (processResult == false)
                    {
                        process.Kill();
                        errors = "Перевищений ліміт виконання (3 сек.).";
                    }
                    else
                        errors = process.StandardError.ReadToEnd();
                    var output = process.StandardOutput.ReadToEnd();


                    return new RuntimeResult(output, errors);
                }
            });
        }
        private async Task<RuntimeResult> Execute(string input)
        {
            if (!isCompiled)
            {
                return new RuntimeResult(string.Empty, "Невдалося знайти виконуючий файл.");
            }

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };
            if (codeFileInfo.Extension == ".py")
            {
                startInfo.FileName = "python";
                startInfo.Arguments = codeFileInfo.FullName;
            }
            else
            {
                var exeFileName = codeFileInfo.FullName.Replace(codeFileInfo.Extension, ".exe");
                startInfo.FileName = exeFileName;
            }
            return await RunProcessAsync(startInfo, input);
        }
        private async Task ProcessTestRun()
        {
            AddNewAction("Пробний запуск виконуючого файлу..");
            var runtimeResult = await Execute(tests[0].InputData);

            if (runtimeResult.IsExecuted)
            {
                progressBarStatus.Value = 20;
                AddNewAction("Пробний запуск пройшов успішно.");
                isTestRunPassed = true;
            }
            else
            {
                progressBarStatus.Foreground = Brushes.MediumVioletRed;
                AddNewAction("Помилка виконання пробного запуску!");
                AddNewAction($"Дані про помилку:\n{runtimeResult.Errors}");
                isTestRunPassed = false;
            }
        }
        private async Task<bool> IsTestPassed(Test test)
        {
            AddNewAction($"Запуск тесту №{testNumber} із {tests.Count}");
            var runtimeResult = await Execute(test.InputData);
            return (runtimeResult.Output.StartsWith(test.OutputData));
        }
        private async Task ProcessTesting()
        {
            TestingAction action;
            double multiplier = 80 / tests.Count;
            foreach (var test in tests)
            {
                if (await IsTestPassed(test))
                {
                    action = AddNewAction($"[Пройдений] Тест №{testNumber}");
                    ChangeRowColor(action, Brushes.Green);
                    passedTests++;
                }
                else
                {
                    action = AddNewAction($"[Провалений] Тест №{testNumber}");
                    ChangeRowColor(action, Brushes.Red);
                    failedTests++;
                }
                testNumber++;
                progressBarStatus.Value += multiplier;
            }
            action = AddNewAction($"Провалено {failedTests} із {tests.Count}");
            ChangeRowColor(action, (failedTests == 0 ? Brushes.DarkGreen : Brushes.Red));
            action = AddNewAction($"Пройдено {passedTests} із {tests.Count}");
            ChangeRowColor(action, Brushes.DarkGreen);
            DataGridDetails.ScrollIntoView(action);
            progressBarStatus.Value = 100;
        }

        private void ChangeRowColor(TestingAction action, Brush color)
        {
            var dgIndex = ActionList.Count - 1;
            var lastRow = (DataGridRow)DataGridDetails.ItemContainerGenerator.ContainerFromIndex(dgIndex);
            if (lastRow == null)
            {
                DataGridDetails.UpdateLayout();
                DataGridDetails.ScrollIntoView(action);
                lastRow = (DataGridRow)DataGridDetails.ItemContainerGenerator.ContainerFromIndex(dgIndex);
            }
            lastRow.Foreground = color;
        }
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            progressBarStatus.Value = 5;
            await ProcessCompilation();
            if (isCompiled)
            {
                await ProcessTestRun();
                if (isTestRunPassed)
                {
                    AddNewAction("Запуск тестування...");
                    testNumber = 1;
                    failedTests = 0;
                    passedTests = 0;
                    await ProcessTesting();
                }
            }
        }

    }
}
