﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using tuc2.DataTypes;

namespace tuc2.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for TaskSolverWnd.xaml
    /// </summary>
    public partial class TaskSolverWnd : UserControl
    {
        private ApplicationContext context;
        private ObservableCollection<string> taskList;
        private string currentDirectory;

        public int SelectedIndexValue { get; set; }

        public TaskSolverWnd()
        {
            context = new ApplicationContext();
            taskList = new ObservableCollection<string>(context.Tasks.Select(t => t.Name));
            if (taskList.Count == 0)
                SelectedIndexValue = -1;
            else
                SelectedIndexValue = 0;
            currentDirectory = Directory.GetCurrentDirectory();

            InitializeComponent();

            DataContext = this;
            this.ListViewTasks.ItemsSource = taskList;
        }

        private void FillOrClearFields(string taskName = "", string description = "", string inputSample = "", string outputSample = "", string codeFile = "")
        {
            this.lbTaskName.Text = taskName;
            this.txtTaskDescription.Text = description;
            this.txtInputSample.Text = inputSample;
            this.txtOutputSample.Text = outputSample;
            this.txtCodeFile.Text = codeFile;
        }
        private void ListViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedIndexValue = this.ListViewTasks.SelectedIndex;
            var taskName = taskList[SelectedIndexValue];
            var task = context.Tasks.SingleOrDefault(t => t.Name == taskName);
            FillOrClearFields(task.Name, task.Description, task.InputExample, task.OutputExample);
            this.lbTaskNumber.Text = $"{SelectedIndexValue + 1} / {taskList.Count}";
            btnNextTask.IsEnabled = !(SelectedIndexValue + 1 == taskList.Count);
            btnPreviusTask.IsEnabled = !(SelectedIndexValue == 0);
        }
        private void BtnNextTask_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndexValue++;
            this.ListViewTasks.SelectedIndex = SelectedIndexValue;
        }
        private void BtnPreviusTask_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndexValue--;
            this.ListViewTasks.SelectedIndex = SelectedIndexValue;
        }
        private void BtnSelectCodeFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDlg = new OpenFileDialog
            {
                Filter = "Pascal (*.pas)|*.pas|Python (*.py)|*.py|C++ (*.cpp)|*.cpp",
                Title = "Оберіть файл із кодом програми для тестування",
                Multiselect = false
            };
            if (openFileDlg.ShowDialog() == true)
            {
                var fileName = new FileInfo(openFileDlg.FileName);
                var codesDir = Path.Combine(currentDirectory, "Codes");
                var distPath = Path.Combine(codesDir, fileName.Name);
                ClearCodeFilesFolder();
                File.Copy(fileName.FullName, distPath);
                this.txtCodeFile.Text = fileName.Name;
            }
            this.btnCheckSolution.IsEnabled = true;
        }
        private void ClearCodeFilesFolder()
        {
            var dir = Path.Combine(currentDirectory, "Codes");
            var dirInfo = new DirectoryInfo(dir);
            foreach (FileInfo file in dirInfo.EnumerateFiles())
            {
                file.Delete();
            }
        }
        private void BtnCheckSolution_Click(object sender, RoutedEventArgs e)
        {
            var taskName = taskList[SelectedIndexValue];
            var task = context.Tasks.SingleOrDefault(t => t.Name == taskName);
            if (task == null)
                return;

            var testingWnd = new TestingWnd(task.Id, this.txtCodeFile.Text);
            testingWnd.ShowDialog();
        }
    }
}
