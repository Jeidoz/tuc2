using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using System.Windows.Shapes;
using tuc2.Entities;

namespace tuc2.Windows.AdminControls
{
    /// <summary>
    /// Interaction logic for TasksCrudWnd.xaml
    /// </summary>
    public partial class TasksCrudWnd : UserControl
    {
        private ApplicationContext context;
        private ObservableCollection<string> taskList;
        private bool isNewTask = false;

        public int SelectedIndexValue { get; set; }


        // TO DO
        // Перевірка на наявність доданих тестів
        public TasksCrudWnd()
        {
            context = new ApplicationContext();
            taskList = new ObservableCollection<string>(context.Tasks.Select(t => t.Name));
            if (taskList.Count == 0)
                isNewTask = true;
            SelectedIndexValue = 0;

            InitializeComponent();
            
            DataContext = this;
            this.ListViewTasks.ItemsSource = taskList;
        }

        private void FillOrClearFields(string taskName = "", string description = "", string inputSample = "", string OutputSample = "", string inputTestFile = "", string OutputTestFile = "")
        {
            this.txtTaskName.Text = taskName;
            this.txtTaskDescription.Text = description;
            this.txtInputSample.Text = inputSample;
            this.txtOutputSample.Text = OutputSample;
        }

        private void ListViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedIndexValue = this.ListViewTasks.SelectedIndex;
            if (SelectedIndexValue == -1)
                SelectedIndexValue = 0;
            var taskName = taskList[SelectedIndexValue];
            var task = context.Tasks.SingleOrDefault(t => t.Name == taskName);
            FillOrClearFields(task.Name, task.Description, task.InputExample, task.OutputExample/*, task.InputFile, task.OutputFile*/);
        }

        private void BtnAddNewTask_Click(object sender, RoutedEventArgs e)
        {
            FillOrClearFields();
            isNewTask = true;
        }

        private bool IsTaskNameReserved(string taskName)
        {
            return context.Tasks.Any(u => u.Name == taskName);
        }

        private bool IsAllFieldsFilled()
        {
            bool name = !string.IsNullOrWhiteSpace(this.txtTaskName.Text);
            bool description = !string.IsNullOrWhiteSpace(this.txtTaskDescription.Text);
            bool inputSample = !string.IsNullOrWhiteSpace(this.txtInputSample.Text);
            bool outputSample = !string.IsNullOrWhiteSpace(this.txtOutputSample.Text);

            return (name && description && inputSample && outputSample);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var oldIndex = this.ListViewTasks.SelectedIndex;
            var taskName = taskList[oldIndex];
            var task = context.Tasks.SingleOrDefault(t => t.Name == taskName);
            context.Tasks.Remove(task);
            context.SaveChanges();
            this.ListViewTasks.SelectedIndex = oldIndex - 1;
            taskList.Remove(taskName);

            var messageText = $"Завдання із іменем {taskName} було успішно видалене із бази даних";
            var messageHeader = "Успішне видалення завдання";
            MessageBox.Show(messageText, messageHeader, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAllFieldsFilled())
            {
                MessageBox.Show("Для збереження завдання необхідно заповнити всі поля і обрати його роль", "Пусті поля", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var newTask = new TestTask()
            {
                Name = this.txtTaskName.Text,
                Description = this.txtTaskDescription.Text,
                InputExample = this.txtInputSample.Text,
                OutputExample = this.txtOutputSample.Text
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
                context.Tasks.Add(newTask);
                context.SaveChanges();
                taskList.Add(newTask.Name);
                isNewTask = false;
            }
            else
            {
                var selectedTaskIndex = this.ListViewTasks.SelectedIndex;
                var selectedTaskName = taskList[selectedTaskIndex];
                var selectedTask = context.Tasks.SingleOrDefault(t => t.Name == selectedTaskName);
                if (selectedTask == null)
                    return;

                selectedTask.Name = newTask.Name;
                selectedTask.Description = newTask.Description;
                selectedTask.InputExample = newTask.InputExample;
                selectedTask.OutputExample = newTask.OutputExample;

                context.SaveChanges();
                taskList[selectedTaskIndex] = selectedTask.Name;
                SelectedIndexValue = selectedTaskIndex;
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
            var isTaskExist = context.Tasks.Any(t => t.Name == taskName);
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
