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
                new TestingAction($"Підготовка до тестування завдання із назвою \"{task.Name}\" завершена.")
            };

            DataContext = this;
            InitializeComponent();

            progressBarStatus.Value = 5;

            ProcessCompilation();
        }
        private void AddNewAction(string action)
        {
            ActionList.Add(new TestingAction(action));
        }

        private void ProcessCompilation()
        {
            AddNewAction("Компіляція коду...");
            var compilationResult = Compile();
            if (compilationResult.IsCompiled)
            {
                progressBarStatus.Value = 10;
                AddNewAction("Компіляція завершена.");
            }
            else
            {
                progressBarStatus.Foreground = Brushes.MediumVioletRed;
                AddNewAction("Помилка компіляції!");
                AddNewAction($"Дані про помилку:\n{compilationResult.Errors}");
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
                CreateNoWindow = false,
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
                startInfo.Arguments = fileName;
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
    }
}
