using System.Collections.ObjectModel;
using System.Windows;
using tuc2.ViewModels;
using Tuc2DDL;

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
            if (testTask.Tests != null)
            {
                foreach (var test in testTask.Tests)
                {
                    TestsList.Add(DataMapper.Map(test));
                }
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

        private void UpdateTests()
        {
            foreach (var test in TestsList)
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

        private void BtnSaveTests_Click(object sender, RoutedEventArgs e)
        {
            var testTask = this.db.GetExercise(taskName);
            UpdateTests();
            testTask.Tests = DataMapper.Map(TestsList);
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
