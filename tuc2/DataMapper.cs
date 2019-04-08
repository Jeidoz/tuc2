using System.Collections.Generic;
using System.Linq;
using tuc2.ViewModels;
using Tuc2DDL.Entities;

namespace tuc2
{
    public class DataMapper
    {
        public static TestViewModel Map(Test test)
        {
            return new TestViewModel
            {
                Id = test.Id,
                InputData = test.InputData,
                OutputData = test.OutputData,
                IsSelected = false
            };
        }
        public static List<TestViewModel> Map(IEnumerable<Test> tests)
        {
            return tests.Select(x => Map(x)).ToList();
            //foreach (var test in tests)
            //    result.Add(Map(test));
            //return result;
        }
        public static Test Map(TestViewModel testViewModel)
        {
            return new Test
            {
                Id = testViewModel.Id,
                InputData = testViewModel.InputData,
                OutputData = testViewModel.OutputData
            };
        }
        public static List<Test> Map(IEnumerable<TestViewModel> testViewModels)
        {
            return testViewModels.Select(x => Map(x)).ToList();
        }
    }
}
