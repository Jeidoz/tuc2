﻿using LiteDB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using tuc2.ViewModels;
using Tuc2DDL;
using Tuc2DDL.Entities;

namespace tuc2.Windows.AdminControls
{
    /// <summary>
    /// Interaction logic for TasksCrudWnd.xaml
    /// </summary>
    public partial class TasksCrudWnd : UserControl
    {
        private DbContext db;
        private ObservableCollection<string> taskList;
        private bool isNewTask = false;

        public int SelectedIndexValue { get; set; }
        public ObservableCollection<TestViewModel> Examples { get; set; }

        public TasksCrudWnd()
        {
            this.db = WpfHelper.Database;
            taskList = new ObservableCollection<string>(db.Exercises.FindAll().Select(x => x.Name));
            if (taskList.Count == 0)
                isNewTask = true;
            SelectedIndexValue = 0;

            InitializeComponent();
            
            DataContext = this;
            this.ListViewTasks.ItemsSource = taskList;
        }

        private void FillOrClearFields(string taskName = "", string description = "", List<TestViewModel> examples = null)
        {
            this.txtTaskName.Text = taskName;
            this.txtTaskDescription.Text = description;
            if (examples == null || examples.Count == 0)
                this.Examples.Clear();
            else
                this.Examples = new ObservableCollection<TestViewModel>(examples);
            this.dataGridExamples.ItemsSource = this.Examples;
        }
        private void ListViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedIndexValue = this.ListViewTasks.SelectedIndex;
            if (SelectedIndexValue == -1)
                SelectedIndexValue = 0;
            var taskName = taskList[SelectedIndexValue];
            var task = this.db.GetExercise(taskName);
            var examples = DataMapper.Map(task.Examples);
            FillOrClearFields(task.Name, task.Description, examples);
        }
        private void BtnAddNewTask_Click(object sender, RoutedEventArgs e)
        {
            FillOrClearFields();
            isNewTask = true;
        }
        private bool IsTaskNameReserved(string taskName)
        {
            return this.db.Exercises.Exists(Query.EQ("Name", taskName));
        }
        private bool IsAllFieldsFilled()
        {
            bool name = !string.IsNullOrWhiteSpace(this.txtTaskName.Text);
            bool description = !string.IsNullOrWhiteSpace(this.txtTaskDescription.Text);
            bool examples = Examples.Count > 0;

            return (name && description && examples);
        }
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var oldIndex = this.ListViewTasks.SelectedIndex;
            var taskName = taskList[oldIndex];
            var task = this.db.GetExercise(taskName);
            this.db.Exercises.Delete(task.Id);
            this.ListViewTasks.SelectedIndex = oldIndex - 1;
            taskList.Remove(taskName);

            var messageText = $"Завдання із іменем {taskName} було успішно видалене із бази даних";
            var messageHeader = "Успішне видалення завдання";
            MessageBox.Show(messageText, messageHeader, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateExampleTests()
        {
            foreach (var test in Examples)
            {
                var searchResult = this.db.Tests.FindById(test.Id);
                var mappedTest = DataMapper.Map(test);
                if (searchResult == null)
                {
                    var dbRecord = mappedTest;
                    test.Id = this.db.Tests.Insert(dbRecord);
                }
                else
                    this.db.Tests.Update(mappedTest);
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAllFieldsFilled())
            {
                MessageBox.Show("Для збереження завдання необхідно заповнити всі поля і обрати його роль", "Пусті поля", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var newTask = new Exercise()
            {
                Name = this.txtTaskName.Text,
                Description = this.txtTaskDescription.Text,
                Examples = DataMapper.Map(this.Examples),
                Files = null,
                Tests = null
            };

            if (isNewTask)
            {
                if (IsTaskNameReserved(newTask.Name))
                {
                    var errorText = $"Завдання із іменем {newTask.Name} уже існує в базі даних";
                    var errorHeader = "Ім'я завдання зайняте";
                    MessageBox.Show(errorText, errorHeader, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }
                UpdateExampleTests();
                newTask.Examples = DataMapper.Map(this.Examples);
                this.db.Exercises.Insert(newTask);
                taskList.Add(newTask.Name);
                isNewTask = false;
            }
            else
            {
                var selectedExerciseIndex = this.ListViewTasks.SelectedIndex;
                var selectedExerciseName = taskList[selectedExerciseIndex];
                var selectedExercise = this.db.GetExercise(selectedExerciseName);
                if (selectedExercise == null)
                    return;

                selectedExercise.Name = newTask.Name;
                selectedExercise.Description = newTask.Description;
                UpdateExampleTests();
                selectedExercise.Examples = DataMapper.Map(this.Examples);

                this.db.Exercises.Update(selectedExercise);
                taskList[selectedExerciseIndex] = selectedExercise.Name;
                SelectedIndexValue = selectedExerciseIndex;
            }

            var index = taskList.IndexOf(newTask.Name);
            this.ListViewTasks.SelectedIndex = index;
            var messageText = $"Завдання із іменем {taskList[SelectedIndexValue]} було успішно збережене у базу даних";
            var messageHeader = "Успішне збереження завдання";
            MessageBox.Show(messageText, messageHeader, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void BtnEditTests_Click(object sender, RoutedEventArgs e)
        {
            var taskName = this.txtTaskName.Text;
            if (string.IsNullOrWhiteSpace(taskName))
            {
                MessageBox.Show("Введіть спочатку назву тесту!", "Відсутня назва тесту", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            var isTaskExist = this.db.IsExerciseExist(taskName);
            if (!isTaskExist)
            {
                MessageBox.Show("Збережіть завдання із початковими данними (Ім'я, Опис, Приклади тестів)", "Не створене завдання", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var wnd = new TestCrudWnd(taskName);
            wnd.ShowDialog();
        }
    }
}
