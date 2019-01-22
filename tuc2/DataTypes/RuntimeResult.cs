using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tuc2.DataTypes
{
    public class RuntimeResult
    {
        public bool IsExecuted { get; set; }
        public string Output { get; set; }
        public string Errors { get; set; }

        public RuntimeResult()
        {
            Errors = string.Empty;
        }

        public RuntimeResult(string output, string errors)
        {
            if (string.IsNullOrWhiteSpace(errors))
            {
                Errors = string.Empty;
                Output = output;
                IsExecuted = true;
            }
            else
            {
                Errors = errors;
                Output = null;
                IsExecuted = false;
            }
        }

        public string PrintTraceback()
        {
            if (IsExecuted)
            {
                Console.WriteLine("Програма була успішно виконана.");
                return string.Empty;
            }
            Console.WriteLine("Відбулась помилка під час виконання:\n");
            Console.WriteLine(Errors);
            return Errors;
        }

        public void PrintOutput()
        {
            if (string.IsNullOrWhiteSpace(Errors))
            {
                Console.WriteLine("Результат роботи програми:");
                Console.WriteLine(Output);
            }
            else
            {
                PrintTraceback();
            }
        }
    }
}
