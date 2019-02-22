using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using tuc2.DataTypes;
using tuc2.Entities;

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

        private ApplicationContext context;
        private TestTask task;
        private List<Test> tests;
        private FileInfo codeFileInfo;
        private bool isCompiled;
        private bool isTestRunPassed;
        private int testNumber;
        private int passedTests;
        private int failedTests;

        public ObservableCollection<TestingAction> ActionList { get; set; }

        public TestingWnd(int taskId, string codeFile)
        {
            context = new ApplicationContext();
            task = context.Tasks.Include(t => t.Tests).Single(t => t.Id == taskId);
            tests = task.Tests;

            var currentDir = Directory.GetCurrentDirectory();
            var distPath = Path.Combine(currentDir, "Codes", codeFile);
            codeFileInfo = new FileInfo(distPath);

            ActionList = new ObservableCollection<TestingAction>()
            {
                new TestingAction("Ініціалізація змінних..."),
                new TestingAction("Зчитування контрольних тестів..."),
                new TestingAction($"Підготовка до тестування завдання із назвою \"{task.Name}\" завершена."),
                new TestingAction("Компіляція коду...")
            };

            DataContext = this;
            InitializeComponent();
        }

        private TestingAction AddNewAction(string action)
        {
            var newAction = new TestingAction(action);
            ActionList.Add(newAction);
            return newAction;
        }
        private void ProcessCompilation()
        {
            
            var compilationResult = Compile();
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
        private CompilationResult Compile()
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
                var exeName = fileName.Replace("." + fileName.Split('.').Last(), "");
                startInfo.Arguments = $" \"{fileName}\" -o \"{exeName}.exe\"";
            }
            else
            {
                startInfo.Arguments = $"\"{fileName}\"";
            }

            var process = Process.Start(startInfo);
            var errors = process.StandardError.ReadToEnd();
            var output = process.StandardOutput.ReadToEnd();
            if (string.IsNullOrWhiteSpace(errors))
                errors = output;
            if (errors.Contains("compiled"))
                errors = string.Empty;
            process.WaitForExit();
            return new CompilationResult(errors);
        }
        private RuntimeResult Execute(string input)
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
            var process = Process.Start(startInfo);
            process.StandardInput.WriteLine(input);
            var output = process.StandardOutput.ReadToEnd();
            var errors = process.StandardError.ReadToEnd();
            process.WaitForExit();
        
            return new RuntimeResult(output, errors);
        }
        private void ProcessTestRun()
        {
            AddNewAction("Пробний запуск виконуючого файлу..");
            var runtimeResult = Execute(tests[0].InputData);
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
        private bool IsTestPassed(Test test)
        {
            AddNewAction($"Запуск тесту №{testNumber} із {tests.Count}");
            var runtimeResult = Execute(test.InputData);
            return (runtimeResult.Output.StartsWith(test.OutputData));
        }
        private void ChangeRowColor(TestingAction action, Brush color)
        {
            var dgIndex = ActionList.Count - 1;
            var lastRow = (DataGridRow)this.DataGridDetails.ItemContainerGenerator.ContainerFromIndex(dgIndex);
            if (lastRow == null)
            {
                this.DataGridDetails.UpdateLayout();
                this.DataGridDetails.ScrollIntoView(action);
                lastRow = (DataGridRow)this.DataGridDetails.ItemContainerGenerator.ContainerFromIndex(dgIndex);
            }
            lastRow.Foreground = color;
        }
        private void ProcessTesting()
        {
            TestingAction action;
            double multiplier = 80 / tests.Count;
            foreach (var test in tests)
            {
                var isTestPassed = IsTestPassed(test);
                if (isTestPassed)
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
                this.progressBarStatus.Value += multiplier;
            }
            action = AddNewAction($"Провалено {failedTests} із {tests.Count}");
            ChangeRowColor(action, (failedTests == 0 ? Brushes.DarkGreen : Brushes.Red));
            action = AddNewAction($"Пройдено {passedTests} із {tests.Count}");
            ChangeRowColor(action, Brushes.DarkGreen);
            this.DataGridDetails.ScrollIntoView(action);
            this.progressBarStatus.Value = 100;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            progressBarStatus.Value = 5;

            ProcessCompilation();
            if (isCompiled)
            {
                ProcessTestRun();
                if (isTestRunPassed)
                {
                    AddNewAction("Запуск тестування...");
                    testNumber = 1;
                    failedTests = 0;
                    passedTests = 0;
                    ProcessTesting();
                }
            }
        }
    }
}
