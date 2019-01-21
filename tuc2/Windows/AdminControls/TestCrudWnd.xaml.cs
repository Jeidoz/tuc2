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
using System.Windows.Shapes;
using tuc2.Entities;

namespace tuc2.Windows.AdminControls
{
    /// <summary>
    /// Interaction logic for TestCrudWnd.xaml
    /// </summary>
    public partial class TestCrudWnd : Window
    {
        private ApplicationContext context;
        private bool isAllItemsSelected;

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
            context = new ApplicationContext();
            TestsList = new ObservableCollection<TestViewModel>();
            var testTask = context.Tasks
                .Include(t => t.Tests)
                .SingleOrDefault(t => t.Name == testName);
            foreach(var test in testTask.Tests)
            {
                TestsList.Add(new TestViewModel()
                {
                    IsSelected = false,
                    InputData = test.InputData,
                    OutputData = test.OutputData
                });
            }

            InitializeComponent();
            
            //TestsList = new ObservableCollection<TestViewModel>()
            //{
            //    new TestViewModel()
            //    {
            //        IsSelected = true,
            //        InputData = "1 1",
            //        OutputData = "2"
            //    },
            //    new TestViewModel()
            //    {
            //        IsSelected = false,
            //        InputData = "-1 1",
            //        OutputData = "0"
            //    }
            //};
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

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
