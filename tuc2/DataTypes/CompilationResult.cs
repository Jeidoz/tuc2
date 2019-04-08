using System;

namespace tuc2.DataTypes
{
    public class CompilationResult
    {
        public bool IsCompiled { get; set; }
        public string Errors { get; set; }

        public CompilationResult()
        {
            IsCompiled = false;
            Errors = string.Empty;
        }
        public CompilationResult(string errors)
        {
            if (string.IsNullOrWhiteSpace(errors))
            {
                Errors = string.Empty;
                IsCompiled = true;
            }
            else
            {
                Errors = errors;
                IsCompiled = false;
            }
        }

        public string PrintTraceback()
        {
            if (IsCompiled)
            {
                Console.WriteLine("Програма була успішно скомпільована.");
                return string.Empty;
            }
            Console.WriteLine("Відбулась помилка компіляції:\n");
            Console.WriteLine(Errors);
            return Errors;
        }
    }
}
