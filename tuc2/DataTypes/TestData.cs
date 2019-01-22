using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tuc2.DataTypes
{
    public class TestData
    {
        public IEnumerable<string> Input { get; set; }
        public string Output { get; set; }

        public TestData()
        {
            Input = new List<string>();
            Output = string.Empty;
        }

        public TestData(string[] input, string output)
        {
            Input = input.ToList();
            Output = output;
        }
    }
}
