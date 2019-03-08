using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using tuc2.ViewModels;
using Tuc2DDL;
using Tuc2DDL.Entities;

namespace tuc2.Windows.AdminControls
{
    /// <summary>
    /// Interaction logic for TestCrudWnd.xaml
    /// </summary>
    public partial class TestCrudWnd : Window
    {
        private DbContext db;
        private bool isAllItemsSelected;
        private string taskName;

        public ObservableCollection<TestViewModel> TestsList { get; set; }
        public bool IsAllItemsSelected
        {
            get { return isAllItemsSelected; }
            set
            {
                if (isAllItemsSelected == value) return;

                isAllItemsSelected = value;
                SelectAll(isAllItemsSelected);
            }
        } 
        
        public TestCrudWnd(string testName)
        {
            DataContext = this;
            this.taskName = testName;
            this.db = WpfHelper.Database;
            TestsList = new ObservableCollection<TestViewModel>();
            var testTask = this.db.GetExercise(testName);
            foreach(var test in testTask.Tests)
            {
                TestsList.Add(DataMapper.Map(test));
            }

            InitializeComponent();
        }

        private void SelectAll(bool select)
        {
            foreach (var test in TestsList)
            {
                test.IsSelected = select;
            }
        }

        private void BtnSaveTests_Click(object sender, RoutedEventArgs e)
        {
            var testTask = this.db.GetExercise(taskName);
            foreach (var test in testTask.Tests)
            {
                this.db.Tests.Delete(Query.EQ("Id", test.Id));
            }
            List<Test> newTests = new List<Test>();
            foreach(var test in TestsList)
            {
                newTests.Add(DataMapper.Map(test));
            }
            testTask.Tests = newTests;
            this.db.Exercises.Update(testTask);

            MessageBox.Show("Тести були успішно збережені у БД", "Тести успішно збережені", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            for (int i = TestsList.Count - 1; i >= 0 ; i--)
            {
                if (TestsList[i].IsSelected)
                {
                    TestsList.RemoveAt(i);
                }
            }
        }
    }
}
